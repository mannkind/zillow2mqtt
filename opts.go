package main

import (
	"reflect"

	"github.com/caarlos0/env/v6"
	"github.com/mannkind/twomqtt"
	log "github.com/sirupsen/logrus"
)

type opts struct {
	General twomqtt.GeneralConfig
	Global  globalOpts
	Sink    sinkOpts
	Source  sourceOpts
}

func newOpts() opts {
	c := opts{
		General: twomqtt.GeneralConfig{},
		Global:  globalOpts{},
		Sink:    sinkOpts{},
		Source:  sourceOpts{},
	}

	// Manually parse the address:name mapping
	if err := env.ParseWithFuncs(&c, map[reflect.Type]env.ParserFunc{
		reflect.TypeOf(sourceMapping{}): twomqtt.SimpleKVMapParser(":", ","),
	}); err != nil {
		log.WithFields(log.Fields{
			"error": err,
		}).Error("Unable to unmarshal configuration")
	}

	// Defaults
	if c.Sink.MQTTOpts.DiscoveryName == "" {
		c.Sink.MQTTOpts.DiscoveryName = "zillow"
	}

	if c.Sink.MQTTOpts.TopicPrefix == "" {
		c.Sink.MQTTOpts.TopicPrefix = "home/zillow"
	}

	// env.Parse* does not seem to work with embedded structs
	c.Sink.ZPIDS = c.Global.ZPIDS
	c.Source.ZPIDS = c.Global.ZPIDS

	if c.General.DebugLogLevel {
		log.SetLevel(log.DebugLevel)
	}

	return c
}
