package source

import (
	"fmt"

	"github.com/jmank88/zillow"
	"github.com/mannkind/zillow2mqtt/shared"
	"github.com/robfig/cron/v3"
	log "github.com/sirupsen/logrus"
)

// Reader is for reading a shared representation out of a source system
type Reader struct {
	opts     Opts
	outgoing chan<- shared.Representation
	service  *Service
}

// NewReader creates a new Reader for reading a shared representation out of a source system
func NewReader(opts Opts, outgoing chan<- shared.Representation, service *Service) *Reader {
	c := Reader{
		opts:     opts,
		outgoing: outgoing,
		service:  service,
	}

	service.SetAPIKey(opts.APIKey)

	return &c
}

// Run starts the Reader
func (c *Reader) Run() {
	// Log service settings
	c.logSettings()

	// Run immediately
	c.poll()

	// Schedule additional runs
	sched := cron.New()
	sched.AddFunc(fmt.Sprintf("@every %s", c.opts.LookupInterval), c.poll)
	sched.Start()
}

// logSettings that are specific to reading the source system
func (c *Reader) logSettings() {
	log.WithFields(log.Fields{
		"Zillow.APIKey":         c.opts.APIKey,
		"Zillow.ZPIDS":          c.opts.ZPIDS,
		"Zillow.LookupInterval": c.opts.LookupInterval,
	}).Info("Service Environmental Settings")
}

// poll the source system, adapt source system responses to the share representation, output data onto a channnel
func (c *Reader) poll() {
	log.Info("Polling")
	for zpid := range c.opts.ZPIDS {
		info, err := c.service.lookup(zpid)
		if err != nil {
			continue
		}

		c.outgoing <- c.adapt(info)
	}

	log.WithFields(log.Fields{
		"sleep": c.opts.LookupInterval,
	}).Info("Finished polling; sleeping")
}

// adapt incoming value(s) to the shared representation
func (c *Reader) adapt(info *zillow.ZestimateResult) shared.Representation {
	return shared.Representation{
		Zpid:   info.Request.Zpid,
		Amount: info.Zestimate.Amount.Value,
	}
}
