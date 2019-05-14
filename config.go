package main

import (
	"log"
	"time"

	"github.com/caarlos0/env"
	mqttExtCfg "github.com/mannkind/paho.mqtt.golang.ext/cfg"
)

type config struct {
	MQTT           *mqttExtCfg.MQTTConfig
	APIKey         string        `env:"ZILLOW_APIKEY,required"`
	ZPIDS          []string      `env:"ZILLOW_ZPIDS" envDefault:""`
	LookupInterval time.Duration `env:"ZILLOW_LOOKUPINTERVAL" envDefault:"24h"`
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
		log.Printf("Error unmarshaling configuration: %s", err)
	}

	redactedPassword := ""
	if len(c.MQTT.Password) > 0 {
		redactedPassword = "<REDACTED>"
	}

	log.Printf("Environmental Settings:")
	log.Printf("  * ClientID        : %s", c.MQTT.ClientID)
	log.Printf("  * Broker          : %s", c.MQTT.Broker)
	log.Printf("  * Username        : %s", c.MQTT.Username)
	log.Printf("  * Password        : %s", redactedPassword)
	log.Printf("  * Discovery       : %t", c.MQTT.Discovery)
	log.Printf("  * DiscoveryPrefix : %s", c.MQTT.DiscoveryPrefix)
	log.Printf("  * DiscoveryName   : %s", c.MQTT.DiscoveryName)
	log.Printf("  * TopicPrefix     : %s", c.MQTT.TopicPrefix)
	log.Printf("  * APIKey          : %s", c.APIKey)
	log.Printf("  * ZPIDS           : %s", c.ZPIDS)
	log.Printf("  * Lookup Interval : %s", c.LookupInterval)
	return &c
}
