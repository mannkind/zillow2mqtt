package main

import (
	"fmt"
	"log"
	"strings"
	"time"

	"github.com/jmank88/zillow"
)

type client struct {
	observers map[observer]struct{}

	apikey         string
	zpids          []string
	lookupInterval time.Duration
}

func newClient(config *config) *client {
	c := client{
		observers: map[observer]struct{}{},

		apikey:         config.APIKey,
		lookupInterval: config.LookupInterval,
	}

	for _, m := range config.ZPIDS {
		parts := strings.Split(m, ":")
		if len(parts) != 2 {
			continue
		}

		zpid := parts[0]
		c.zpids = append(c.zpids, zpid)
	}

	return &c
}

func (c *client) run() {
	go c.loop(false)
}

func (c *client) register(l observer) {
	c.observers[l] = struct{}{}
}

func (c *client) publish(e event) {
	for o := range c.observers {
		o.receive(e)
	}
}

func (c *client) loop(once bool) {
	for {
		log.Print("Beginning lookup")
		for _, zpid := range c.zpids {
			if info, err := c.lookup(zpid); err == nil {
				c.publish(event{
					version: 1,
					data:    c.adapt(info),
				})
			} else {
				log.Print(err)
			}
		}
		log.Print("Ending lookup")

		if once {
			break
		}

		time.Sleep(c.lookupInterval)
	}
}

func (c *client) lookup(zpid string) (*zillow.ZestimateResult, error) {
	client := zillow.New(c.apikey)

	result, err := client.GetZestimate(zillow.ZestimateRequest{Zpid: zpid})
	if err != nil {
		log.Print(err)
		return &zillow.ZestimateResult{}, fmt.Errorf("Unable to fetch information")
	}

	return result, nil
}

func (c *client) adapt(info *zillow.ZestimateResult) eventData {
	return eventData{
		Zpid:   info.Request.Zpid,
		Amount: info.Zestimate.Amount.Value,
	}
}
