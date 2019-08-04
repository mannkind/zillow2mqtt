package main

import (
	"fmt"
	"reflect"
	"strings"
	"time"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	mqttExtDI "github.com/mannkind/paho.mqtt.golang.ext/di"
	mqttExtHA "github.com/mannkind/paho.mqtt.golang.ext/ha"
	log "github.com/sirupsen/logrus"
)

const (
	sensorTopicTemplate = "%s/%s/state"
)

type mqttClient struct {
	discovery       bool
	discoveryPrefix string
	discoveryName   string
	topicPrefix     string

	zpidMapping map[string]string

	client        mqtt.Client
	lastPublished map[string]string
}

func newMQTTClient(config *config, mqttFuncWrapper *mqttExtDI.MQTTFuncWrapper) *mqttClient {
	c := mqttClient{
		discovery:       config.MQTT.Discovery,
		discoveryPrefix: config.MQTT.DiscoveryPrefix,
		discoveryName:   config.MQTT.DiscoveryName,
		topicPrefix:     config.MQTT.TopicPrefix,

		zpidMapping:   map[string]string{},
		lastPublished: map[string]string{},
	}

	// Create a mapping between zpid and name
	for _, m := range config.ZPIDS {
		parts := strings.Split(m, ":")
		if len(parts) != 2 {
			continue
		}

		zpid := parts[0]
		name := parts[1]
		c.zpidMapping[zpid] = name
	}

	opts := mqttFuncWrapper.
		ClientOptsFunc().
		AddBroker(config.MQTT.Broker).
		SetClientID(config.MQTT.ClientID).
		SetOnConnectHandler(c.onConnect).
		SetConnectionLostHandler(c.onDisconnect).
		SetUsername(config.MQTT.Username).
		SetPassword(config.MQTT.Password).
		SetWill(c.availabilityTopic(), "offline", 0, true)

	c.client = mqttFuncWrapper.ClientFunc(opts)

	return &c
}

func (c *mqttClient) run() {
	c.runAfter(0 * time.Second)
}

func (c *mqttClient) runAfter(delay time.Duration) {
	time.Sleep(delay)

	log.Info("Connecting to MQTT")
	if token := c.client.Connect(); !token.Wait() || token.Error() != nil {
		log.WithFields(log.Fields{
			"error": token.Error(),
		}).Error("Error connecting to MQTT")

		delay = c.adjustReconnectDelay(delay)

		log.WithFields(log.Fields{
			"delay": delay,
		}).Info("Sleeping before attempting to reconnect to MQTT")

		c.runAfter(delay)
	}
}

func (c *mqttClient) adjustReconnectDelay(delay time.Duration) time.Duration {
	var maxDelay float64 = 120
	defaultDelay := 2 * time.Second

	// No delay, set to default delay
	if delay.Seconds() == 0 {
		delay = defaultDelay
	} else {
		// Increment the delay
		delay = delay * 2

		// If the delay is above two minutes, reset to default
		if delay.Seconds() > maxDelay {
			delay = defaultDelay
		}
	}

	return delay
}

func (c *mqttClient) onConnect(client mqtt.Client) {
	log.Info("Connected to MQTT")
	c.publish(c.availabilityTopic(), "online")
	c.publishDiscovery()
}

func (c *mqttClient) onDisconnect(client mqtt.Client, err error) {
	log.WithFields(log.Fields{
		"error": err,
	}).Error("Disconnected from MQTT")
}

func (c *mqttClient) availabilityTopic() string {
	return fmt.Sprintf("%s/status", c.topicPrefix)
}

func (c *mqttClient) sensorSlug(name string, sensor string, sep string) string {
	return strings.ToLower(fmt.Sprintf("%s%s%s", name, sep, sensor))
}

func (c *mqttClient) publishDiscovery() {
	if !c.discovery {
		return
	}

	for _, name := range c.zpidMapping {
		obj := reflect.ValueOf(eventData{})
		for i := 0; i < obj.NumField(); i++ {
			field := obj.Type().Field(i)
			sensor := strings.ToLower(field.Name)
			sensorOverride := field.Tag.Get("mqtt")
			sensorType := field.Tag.Get("mqttDiscoveryType")

			// Skip any fields tagged as ignored for mqtt
			if strings.Contains(sensorOverride, ",ignore") {
				continue
			}

			// Override sensor name
			if sensorOverride != "" {
				sensor = sensorOverride
			}

			// Skip any fields tagged as ignores for discovery
			if strings.Contains(sensorType, ",ignore") {
				continue
			}

			underscoredSensor := c.sensorSlug(name, sensor, "_")
			periodSensor := c.sensorSlug(name, sensor, ".")
			spaceSensor := c.sensorSlug(name, sensor, " ")

			mqd := mqttExtHA.MQTTDiscovery{
				DiscoveryPrefix: c.discoveryPrefix,
				Component:       sensorType,
				NodeID:          c.discoveryName,
				ObjectID:        underscoredSensor,

				AvailabilityTopic: c.availabilityTopic(),
				Name:              spaceSensor,
				StateTopic:        fmt.Sprintf(sensorTopicTemplate, c.topicPrefix, underscoredSensor),
				UniqueID:          fmt.Sprintf("%s.%s", c.discoveryName, periodSensor),
				Icon:              "mdi:home-variant",
				UnitOfMeasurement: "$",
			}

			mqd.PublishDiscovery(c.client)
		}
	}
}

func (c *mqttClient) receiveCommand(cmd int64, e event) {}
func (c *mqttClient) receiveState(e event) {
	info := e.data
	name := c.zpidMapping[info.Zpid]

	obj := reflect.ValueOf(info)
	for i := 0; i < obj.NumField(); i++ {
		field := obj.Type().Field(i)
		val := obj.Field(i)
		sensor := strings.ToLower(field.Name)
		sensorOverride := field.Tag.Get("mqtt")
		sensorType := field.Tag.Get("mqttDiscoveryType")

		// Skip any fields tagged as ignored for mqtt
		if strings.Contains(sensorOverride, ",ignore") {
			continue
		}

		// Override sensor name
		if sensorOverride != "" {
			sensor = sensorOverride
		}

		// Skip any fields tagged as ignores for discovery
		if strings.Contains(sensorType, ",ignore") {
			continue
		}

		topic := fmt.Sprintf(sensorTopicTemplate, c.topicPrefix, c.sensorSlug(name, sensor, "_"))
		payload := ""

		switch val.Kind() {
		case reflect.Bool:
			payload = "OFF"
			if val.Bool() {
				payload = "ON"
			}
		case reflect.Int:
			payload = fmt.Sprintf("%d", val.Int())
		}

		if payload == "" {
			continue
		}

		c.publish(topic, payload)
	}
}

func (c *mqttClient) publish(topic string, payload string) {
	llog := log.WithFields(log.Fields{
		"topic":   topic,
		"payload": payload,
	})
	// Should we publish this again?
	// NOTE: We must allow the availability topic to publish duplicates
	if lastPayload, ok := c.lastPublished[topic]; topic != c.availabilityTopic() && ok && lastPayload == payload {
		llog.Debug("Duplicate payload")
		return
	}

	llog.Info("Publishing to MQTT")

	retain := true
	if token := c.client.Publish(topic, 0, retain, payload); token.Wait() && token.Error() != nil {
		log.Error("Publishing error")
	}

	llog.Debug("Published to MQTT")
	c.lastPublished[topic] = payload
}

func intSliceContains(s []int, e int) bool {
	for _, a := range s {
		if a == e {
			return true
		}
	}
	return false
}
