package main

import (
	"time"

	"github.com/caarlos0/env"
	mqttExtCfg "github.com/mannkind/paho.mqtt.golang.ext/cfg"
	log "github.com/sirupsen/logrus"
)

type config struct {
	MQTT           *mqttExtCfg.MQTTConfig
	APIKey         string        `env:"ZILLOW_APIKEY,required"`
	ZPIDS          []string      `env:"ZILLOW_ZPIDS" envDefault:""`
	LookupInterval time.Duration `env:"ZILLOW_LOOKUPINTERVAL" envDefault:"24h"`
	DebugLogLevel  bool          `env:"ZILLOW_DEBUG" envDefault:"false"`
}

func newConfig(mqttCfg *mqttExtCfg.MQTTConfig) *config {
	c := config{}
	c.MQTT = mqttCfg
	c.MQTT.Defaults("DefaultZillow2MqttClientID", "zillow", "home/zillow")

	if err := env.Parse(&c); err != nil {
		log.WithFields(log.Fields{
			"error": err,
		}).Error("Unable to unmarshal configuration")
	}

	log.WithFields(log.Fields{
		"Zillow.APIKey":         c.APIKey,
		"Zillow.ZPIDS":          c.ZPIDS,
		"Zillow.LookupInterval": c.LookupInterval,
	}).Info("Environmental Settings")

	if c.DebugLogLevel {
		log.SetLevel(log.DebugLevel)
		log.Debug("Enabling the debug log level")
	}

	return &c
}
