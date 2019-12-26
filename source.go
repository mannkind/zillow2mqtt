package main

import (
	"fmt"

	"github.com/jmank88/zillow"
	"github.com/robfig/cron/v3"
	log "github.com/sirupsen/logrus"
)

type source struct {
	config   sourceOpts
	outgoing chan<- sourceRep
}

func newSource(config sourceOpts, outgoing chan<- sourceRep) *source {
	c := source{
		config:   config,
		outgoing: outgoing,
	}

	return &c
}

func (c *source) run() {
	// Log service settings
	c.logSettings()

	// Run immediately
	c.poll()

	// Schedule additional runs
	sched := cron.New()
	sched.AddFunc(fmt.Sprintf("@every %s", c.config.LookupInterval), c.poll)
	sched.Start()
}

func (c *source) logSettings() {
	log.WithFields(log.Fields{
		"Zillow.APIKey":         c.config.APIKey,
		"Zillow.ZPIDS":          c.config.ZPIDS,
		"Zillow.LookupInterval": c.config.LookupInterval,
	}).Info("Service Environmental Settings")
}

func (c *source) poll() {
	log.Info("Polling")
	for zpid := range c.config.ZPIDS {
		info, err := c.lookup(zpid)
		if err != nil {
			continue
		}

		c.outgoing <- c.adapt(info)
	}

	log.WithFields(log.Fields{
		"sleep": c.config.LookupInterval,
	}).Info("Finished polling; sleeping")
}

func (c *source) lookup(zpid string) (*zillow.ZestimateResult, error) {
	log.WithFields(log.Fields{
		"zpid": zpid,
	}).Info("Lookup zestimate")

	client := zillow.New(c.config.APIKey)

	result, err := client.GetZestimate(zillow.ZestimateRequest{Zpid: zpid})
	if err != nil {
		log.WithFields(log.Fields{
			"error": err,
			"zpid":  zpid,
		}).Error("Unable to look up zestimate")

		return &zillow.ZestimateResult{}, err
	}

	log.Debug("Finished zestimate lookup")
	return result, nil
}

func (c *source) adapt(info *zillow.ZestimateResult) sourceRep {
	return sourceRep{
		Zpid:   info.Request.Zpid,
		Amount: info.Zestimate.Amount.Value,
	}
}
