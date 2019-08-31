package main

import (
	"reflect"
	"time"

	"github.com/jmank88/zillow"
	"github.com/mannkind/twomqtt"
	log "github.com/sirupsen/logrus"
)

type serviceClient struct {
	twomqtt.Publisher
	serviceClientConfig
	observers map[twomqtt.Observer]struct{}
}

func newServiceClient(serviceClientCfg serviceClientConfig) *serviceClient {
	c := serviceClient{
		serviceClientConfig: serviceClientCfg,
		observers:           map[twomqtt.Observer]struct{}{},
	}

	log.WithFields(log.Fields{
		"Zillow.APIKey":         c.APIKey,
		"Zillow.ZPIDS":          c.ZPIDS,
		"Zillow.LookupInterval": c.LookupInterval,
	}).Info("Service Environmental Settings")

	return &c
}

func (c *serviceClient) run() {
	go c.loop()
}

func (c *serviceClient) Register(l twomqtt.Observer) {
	c.observers[l] = struct{}{}
}

func (c *serviceClient) sendState(e twomqtt.Event) {
	log.WithFields(log.Fields{
		"event": e,
	}).Debug("Sending event to observers")

	for o := range c.observers {
		o.ReceiveState(e)
	}

	log.Debug("Finished sending event to observers")
}

func (c *serviceClient) loop() {
	for {
		log.Info("Looping")
		for zpid := range c.ZPIDS {
			info, err := c.lookup(zpid)
			if err != nil {
				continue
			}

			event, err := c.adapt(info)
			if err != nil {
				continue
			}

			c.sendState(event)
		}

		log.WithFields(log.Fields{
			"sleep": c.LookupInterval,
		}).Info("Finished looping; sleeping")
		time.Sleep(c.LookupInterval)
	}
}

func (c *serviceClient) lookup(zpid string) (*zillow.ZestimateResult, error) {
	log.WithFields(log.Fields{
		"zpid": zpid,
	}).Info("Looking up zestimate")

	client := zillow.New(c.APIKey)

	result, err := client.GetZestimate(zillow.ZestimateRequest{Zpid: zpid})
	if err != nil {
		log.WithFields(log.Fields{
			"error": err,
			"zpid":  zpid,
		}).Error("Unable to look up zestimate")

		return &zillow.ZestimateResult{}, err
	}

	log.Debug("Finished looking up zestimate")
	return result, nil
}

func (c *serviceClient) adapt(info *zillow.ZestimateResult) (twomqtt.Event, error) {
	log.WithFields(log.Fields{
		"onfi": info,
	}).Debug("Adapting zestimate information")

	obj := zestimate{
		Zpid:   info.Request.Zpid,
		Amount: info.Zestimate.Amount.Value,
	}

	event := twomqtt.Event{
		Type:    reflect.TypeOf(obj),
		Payload: obj,
	}

	log.Debug("Finished adapting zestimate information")
	return event, nil
}
