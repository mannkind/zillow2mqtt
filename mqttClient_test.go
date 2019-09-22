package main

import (
	"os"
	"testing"

	log "github.com/sirupsen/logrus"
)

func init() {
	log.SetLevel(log.PanicLevel)
}

func setEnvs(d, dn, tp, a string) {
	os.Setenv("MQTT_DISCOVERY", d)
	os.Setenv("MQTT_DISCOVERYNAME", dn)
	os.Setenv("MQTT_TOPICPREFIX", tp)
	os.Setenv("ZILLOW_APIKEY", "")
	os.Setenv("ZILLOW_ZPIDS", a)
}

func clearEnvs() {
	setEnvs("false", "", "", "")
}

const defaultDiscoveryName = "zillow"
const defaultTopicPrefix = "home/zillow"
const knownZPID = "15678993"
const knownZPIDName = "myhouse"
const knownDiscoveryName = "zillowDiscoveryName"
const knownTopicPrefix = "home/zillowMQTTTopicPrefix"

func TestDiscovery(t *testing.T) {
	defer clearEnvs()

	var tests = []struct {
		ZPIDS           string
		DiscoveryName   string
		TopicPrefix     string
		ExpectedTopic   string
		ExpectedPayload string
	}{
		{
			knownZPID,
			defaultDiscoveryName,
			defaultTopicPrefix,
			"homeassistant/sensor/" + defaultDiscoveryName + "/zestimate/config",
			"{\"availability_topic\":\"" + defaultTopicPrefix + "/status\",\"device\":{\"identifiers\":[\"" + defaultTopicPrefix + "/status\"],\"manufacturer\":\"twomqtt\",\"name\":\"x2mqtt\",\"sw_version\":\"X.X.X\"},\"icon\":\"mdi:home-variant\",\"name\":\"" + defaultDiscoveryName + " zestimate\",\"state_topic\":\"" + defaultTopicPrefix + "/zestimate/state\",\"unique_id\":\"zillow.zestimate\",\"unit_of_measurement\":\"$\"}",
		},
		{
			knownZPID + ":" + knownZPIDName,
			knownDiscoveryName,
			knownTopicPrefix,
			"homeassistant/sensor/" + knownDiscoveryName + "/" + knownZPIDName + "_zestimate/config",
			"{\"availability_topic\":\"" + knownTopicPrefix + "/status\",\"device\":{\"identifiers\":[\"" + knownTopicPrefix + "/status\"],\"manufacturer\":\"twomqtt\",\"name\":\"x2mqtt\",\"sw_version\":\"X.X.X\"},\"icon\":\"mdi:home-variant\",\"name\":\"" + knownDiscoveryName + " " + knownZPIDName + " zestimate\",\"state_topic\":\"" + knownTopicPrefix + "/" + knownZPIDName + "/zestimate/state\",\"unique_id\":\"" + knownDiscoveryName + "." + knownZPIDName + ".zestimate\",\"unit_of_measurement\":\"$\"}",
		},
	}

	for _, v := range tests {
		setEnvs("true", v.DiscoveryName, v.TopicPrefix, v.ZPIDS)

		c := initialize()
		c.mqttClient.publishDiscovery()

		actualPayload := c.mqttClient.LastPublishedOnTopic(v.ExpectedTopic)
		if actualPayload != v.ExpectedPayload {
			t.Errorf("Actual:%s\nExpected:%s", actualPayload, v.ExpectedPayload)
		}
	}
}

func TestReceieveState(t *testing.T) {
	defer clearEnvs()

	var tests = []struct {
		ZPIDS           string
		ZPid            string
		ActualAmount    int
		TopicPrefix     string
		ExpectedTopic   string
		ExpectedPayload string
	}{
		{
			knownZPID,
			knownZPID,
			652721,
			defaultTopicPrefix,
			defaultTopicPrefix + "/zestimate/state",
			"652721",
		},
		{
			knownZPID + ":" + knownZPIDName,
			knownZPID,
			652721,
			knownTopicPrefix,
			knownTopicPrefix + "/" + knownZPIDName + "/zestimate/state",
			"652721",
		},
	}

	for _, v := range tests {
		setEnvs("false", "", v.TopicPrefix, v.ZPIDS)

		obj := zestimate{
			Zpid:   v.ZPid,
			Amount: v.ActualAmount,
		}

		c := initialize()
		c.mqttClient.receiveState(obj)

		actualPayload := c.mqttClient.LastPublishedOnTopic(v.ExpectedTopic)
		if actualPayload != v.ExpectedPayload {
			t.Errorf("Actual:%s\nExpected:%s", actualPayload, v.ExpectedPayload)
		}
	}
}
