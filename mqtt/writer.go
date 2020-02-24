package mqtt

import (
	"fmt"
	"reflect"
	"strings"

	"github.com/mannkind/twomqtt"
	"github.com/mannkind/zillow2mqtt/shared"
)

// Writer is for writing a shared representation to MQTT
type Writer struct {
	*twomqtt.MQTT
	opts     Opts
	incoming <-chan shared.Representation
}

// NewWriter creates a new Writer for writing a shared representation to MQTT
func NewWriter(mqtt *twomqtt.MQTT, opts Opts, incoming <-chan shared.Representation) *Writer {
	c := Writer{
		MQTT:     mqtt,
		opts:     opts,
		incoming: incoming,
	}

	c.MQTT.
		SetDiscoveryHandler(c.discovery).
		SetReadIncomingChannelHandler(c.read).
		Initialize()

	return &c
}

func (c *Writer) discovery() []twomqtt.MQTTDiscovery {
	mqds := []twomqtt.MQTTDiscovery{}
	if !c.Discovery {
		return mqds
	}

	for _, deviceName := range c.opts.ZPIDS {
		obj := reflect.ValueOf(shared.Representation{})
		for i := 0; i < obj.NumField(); i++ {
			field := obj.Type().Field(i)
			sensorName := strings.ToLower(field.Name)
			sensorOverride, sensorIgnored := twomqtt.MQTTOverride(field)
			sensorType, sensorTypeIgnored := twomqtt.MQTTDiscoveryOverride(field)

			// Skip any fields tagged as ignored
			if sensorIgnored || sensorTypeIgnored {
				continue
			}

			// Override sensor name
			if sensorOverride != "" {
				sensorName = sensorOverride
			}

			mqd := twomqtt.NewMQTTDiscovery(c.opts.MQTTOpts, deviceName, sensorName, sensorType)
			mqd.Icon = "mdi:home-variant"
			mqd.UnitOfMeasurement = "$"
			mqd.Device.Name = shared.Name
			mqd.Device.SWVersion = shared.Version

			mqds = append(mqds, *mqd)
		}
	}

	return mqds
}

// read incoming shared representations and publish them to MQTT
func (c *Writer) read() {
	for info := range c.incoming {
		c.publish(info)
	}
}

// publish a shared representation to MQTT
func (c *Writer) publish(info shared.Representation) twomqtt.MQTTMessage {
	deviceName := c.opts.ZPIDS[info.Zpid]
	sensorName := "Amount"

	topic := c.StateTopic(deviceName, sensorName)
	payload := fmt.Sprintf("%d", info.Amount)

	return c.Publish(topic, payload)
}
