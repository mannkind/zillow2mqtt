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
		ZPIDS                 string
		DiscoveryName         string
		TopicPrefix           string
		ExpectedName          string
		ExpectedStateTopic    string
		ExpectedUniqueID      string
		ExpectedIcon          string
		ExpectedUnitOfMeasure string
	}{
		{
			knownZPID,
			defaultDiscoveryName,
			defaultTopicPrefix,
			defaultDiscoveryName + " zestimate",
			defaultTopicPrefix + "/zestimate/state",
			"zillow.zestimate",
			"mdi:home-variant",
			"$",
		},
		{
			knownZPID + ":" + knownZPIDName,
			knownDiscoveryName,
			knownTopicPrefix,
			knownDiscoveryName + " " + knownZPIDName + " zestimate",
			knownTopicPrefix + "/" + knownZPIDName + "/zestimate/state",
			knownDiscoveryName + "." + knownZPIDName + ".zestimate",
			"mdi:home-variant",
			"$",
		},
	}

	for _, v := range tests {
		setEnvs("true", v.DiscoveryName, v.TopicPrefix, v.ZPIDS)

		c := initialize()
		mqds := c.sink.discovery()

		for _, mqd := range mqds {
			if mqd.Name != v.ExpectedName {
				t.Errorf("discovery Name does not match; %s vs %s", mqd.Name, v.ExpectedName)
			}
			if mqd.StateTopic != v.ExpectedStateTopic {
				t.Errorf("discovery StateTopic does not match; %s vs %s", mqd.StateTopic, v.ExpectedStateTopic)
			}
			if mqd.UniqueID != v.ExpectedUniqueID {
				t.Errorf("discovery UniqueID does not match; %s vs %s", mqd.UniqueID, v.ExpectedUniqueID)
			}
			if mqd.Icon != v.ExpectedIcon {
				t.Errorf("discovery Icon does not match; %s vs %s", mqd.Icon, v.ExpectedIcon)
			}
			if mqd.UnitOfMeasurement != v.ExpectedUnitOfMeasure {
				t.Errorf("discovery UnitOfMeasurement does not match; %s vs %s", mqd.UnitOfMeasurement, v.ExpectedUnitOfMeasure)
			}
		}
	}
}

func TestPublish(t *testing.T) {
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

		obj := sourceRep{
			Zpid:   v.ZPid,
			Amount: v.ActualAmount,
		}

		c := initialize()
		publishedState := c.sink.publish(obj)

		actualPayload := publishedState.Payload
		if actualPayload != v.ExpectedPayload {
			t.Errorf("Actual:%s\nExpected:%s", actualPayload, v.ExpectedPayload)
		}
	}
}
