# zillow2mqtt

[![Software
License](https://img.shields.io/badge/License-MIT-orange.svg?style=flat-square)](https://github.com/mannkind/zillow2mqtt/blob/master/LICENSE.md)
[![Build Status](https://github.com/mannkind/zillow2mqtt/workflows/Main%20Workflow/badge.svg)](https://github.com/mannkind/zillow2mqtt/actions)
[![Coverage Status](https://img.shields.io/codecov/c/github/mannkind/zillow2mqtt/master.svg)](http://codecov.io/github/mannkind/zillow2mqtt?branch=master)
[![Go Report Card](https://goreportcard.com/badge/github.com/mannkind/zillow2mqtt)](https://goreportcard.com/report/github.com/mannkind/zillow2mqtt)

An experiment to publish Zillow ZEstimates to MQTT.

See also Zillow's API documentation at <http://www.zillow.com/howto/api/APIOverview.htm>

## Use

The application can be locally built using `mage` or you can utilize the multi-architecture Docker image(s).

### Example

```bash
docker run \
-e ZILLOW_APIKEY="B1-AWz18xy032zklA_6Nmn1" \
-e ZILLOW_ZPIDS="69103754:MyAddress" \
-e MQTT_BROKER="tcp://localhost:1883" \
-e MQTT_DISCOVERY="true" \
mannkind/unifi2mqtt:latest
```

OR

```bash
ZILLOW_APIKEY="B1-AWz18xy032zklA_6Nmn1" \
ZILLOW_ZPIDS="69103754:MyAddress" \
MQTT_BROKER="tcp://localhost:1883" \
MQTT_DISCOVERY="true" \
./zillow2mqtt 
```

## Environment Variables

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
