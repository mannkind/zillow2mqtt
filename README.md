# zillow2mqtt

[![Software
License](https://img.shields.io/badge/License-MIT-orange.svg?style=flat-square)](https://github.com/mannkind/zillow2mqtt/blob/master/LICENSE.md)
[![Travis CI](https://img.shields.io/travis/mannkind/zillow2mqtt/master.svg?style=flat-square)](https://travis-ci.org/mannkind/zillow2mqtt)
[![Coverage Status](https://img.shields.io/codecov/c/github/mannkind/zillow2mqtt/master.svg)](http://codecov.io/github/mannkind/zillow2mqtt?branch=master)
[![Go Report Card](https://goreportcard.com/badge/github.com/mannkind/zillow2mqtt)](https://goreportcard.com/report/github.com/mannkind/zillow2mqtt)

See also Zillow's API documentation at <http://www.zillow.com/howto/api/APIOverview.htm>

## Installation

### Via Docker

```bash
docker run -d --name="zillow2mqtt" -e "ZILLOW_APIKEY=1234567890" ZILLOW_ZPIDS="5454:Home,54654654:VacationProperty" -v /etc/localtime:/etc/localtime:ro mannkind/zillow2mqtt
```

### Via Make

```bash
git clone https://github.com/mannkind/zillow2mqtt
cd zillow2mqtt
make
ZILLOW_APIKEY="1234567890" ZILLOW_ZPIDS="5454:Home,54654654:VacationProperty" ./zillow2mqtt
```

## Configuration

Configuration happens via environmental variables

```bash
ZILLOW_APIKEY               - The api key for zillow
ZILLOW_ZPID                 - The comma separated zpid:name pairs, defaults to ""
ZILLOW_LOOKUPINTERVAL       - The duration to wait before looking up the zestimate again, defaults to "24h"
MQTT_TOPICPREFIX            - [OPTIONAL] The MQTT topic on which to publish the lookup results, defaults to "home/zillow"
MQTT_DISCOVERY              - [OPTIONAL] The MQTT discovery flag for Home Assistant, defaults to false
MQTT_DISCOVERYPREFIX        - [OPTIONAL] The MQTT discovery prefix for Home Assistant, defaults to "homeassistant"
MQTT_DISCOVERYNAME          - [OPTIONAL] The MQTT discovery name for Home Assistant, defaults to "zillow"
MQTT_CLIENTID               - [OPTIONAL] The clientId, defaults to ""
MQTT_BROKER                 - [OPTIONAL] The MQTT broker, defaults to "tcp://mosquitto.org:1883"
MQTT_USERNAME               - [OPTIONAL] The MQTT username, default to ""
MQTT_PASSWORD               - [OPTIONAL] The MQTT password, default to ""
```
