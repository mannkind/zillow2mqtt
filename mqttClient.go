package main

import (
	"fmt"
	"log"
	"strings"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	mqttExtDI "github.com/mannkind/paho.mqtt.golang.ext/di"
	mqttExtHA "github.com/mannkind/paho.mqtt.golang.ext/ha"
)

const (
	sensorTopicTemplate = "%s/%s/state"
)

// mqttClient - Lookup collection information on seattle.gov.
type mqttClient struct {
	discovery       bool
	discoveryPrefix string
	discoveryName   string
	topicPrefix     string

	zpidMapping map[string]string

	client mqtt.Client
}

func newMQTTClient(config *config, mqttFuncWrapper *mqttExtDI.MQTTFuncWrapper) *mqttClient {
	c := mqttClient{
		discovery:       config.MQTT.Discovery,
		discoveryPrefix: config.MQTT.DiscoveryPrefix,
		discoveryName:   config.MQTT.DiscoveryName,
		topicPrefix:     config.MQTT.TopicPrefix,
	}

	c.zpidMapping = make(map[string]string, 0)

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
	log.Print("Connecting to MQTT")
	if token := c.client.Connect(); !token.Wait() || token.Error() != nil {
		log.Printf("Error connecting to MQTT: %s", token.Error())
		panic("Exiting...")
	}
}

func (c *mqttClient) onConnect(client mqtt.Client) {
	log.Print("Connected to MQTT")
	c.publish(c.availabilityTopic(), "online")
	c.publishDiscovery()
}

func (c *mqttClient) onDisconnect(client mqtt.Client, err error) {
	log.Printf("Disconnected from MQTT: %s.", err)
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
		sensor := "zestimate"
		sensorType := "sensor"

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

func (c *mqttClient) receive(e event) {
	info := e.data

	sensor := "zestimate"
	name := c.zpidMapping[info.Zpid]
	topic := fmt.Sprintf(sensorTopicTemplate, c.topicPrefix, c.sensorSlug(name, sensor, "_"))
	payload := fmt.Sprintf("%d", info.Amount)

	if payload == "" {
		return
	}

	c.publish(topic, payload)
}

func (c *mqttClient) publish(topic string, payload string) {
	retain := true
	if token := c.client.Publish(topic, 0, retain, payload); token.Wait() && token.Error() != nil {
		log.Printf("Publish Error: %s", token.Error())
	}

	log.Print(fmt.Sprintf("Publishing - Topic: %s ; Payload: %s", topic, payload))
}

func intSliceContains(s []int, e int) bool {
	for _, a := range s {
		if a == e {
			return true
		}
	}
	return false
}
