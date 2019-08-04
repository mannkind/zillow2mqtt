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

	if c.MQTT.ClientID == "" {
		c.MQTT.ClientID = "DefaultZillow2MqttClientID"
	}

	if c.MQTT.DiscoveryName == "" {
		c.MQTT.DiscoveryName = "zillow"
	}

	if c.MQTT.TopicPrefix == "" {
		c.MQTT.TopicPrefix = "home/zillow"
	}

	if err := env.Parse(&c); err != nil {
		log.WithFields(log.Fields{
			"error": err,
		}).Error("Unable to unmarshal configuration")
	}

	redactedPassword := ""
	if len(c.MQTT.Password) > 0 {
		redactedPassword = "<REDACTED>"
	}

	log.WithFields(log.Fields{
		"MQTT.ClientID":         c.MQTT.ClientID,
		"MQTT.Broker":           c.MQTT.Broker,
		"MQTT.Username":         c.MQTT.Username,
		"MQTT.Password":         redactedPassword,
		"MQTT.Discovery":        c.MQTT.Discovery,
		"MQTT.DiscoveryPrefix":  c.MQTT.DiscoveryPrefix,
		"MQTT.DiscoveryName":    c.MQTT.DiscoveryName,
		"MQTT.TopicPrefix":      c.MQTT.TopicPrefix,
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
