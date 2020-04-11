# zillow2mqtt

[![Software
License](https://img.shields.io/badge/License-MIT-orange.svg?style=flat-square)](https://github.com/mannkind/zillow2mqtt/blob/master/LICENSE.md)
[![Build Status](https://github.com/mannkind/zillow2mqtt/workflows/Main%20Workflow/badge.svg)](https://github.com/mannkind/zillow2mqtt/actions)
[![Coverage Status](https://img.shields.io/codecov/c/github/mannkind/zillow2mqtt/master.svg)](http://codecov.io/github/mannkind/zillow2mqtt?branch=master)

An experiment to publish Zillow ZEstimates to MQTT.

See also Zillow's API documentation at <http://www.zillow.com/howto/api/APIOverview.htm>

## Use

The application can be locally built using `dotnet build` or you can utilize the multi-architecture Docker image(s).

### Example

```bash
docker run \
-e ZILLOW__SOURCE__APIKEY="B1-AWz18xy032zklA_6Nmn1" \
-e ZILLOW__SHARED__RESOURCES__0__ZPID="69103754" \
-e ZILLOW__SHARED__RESOURCES__0__Slug="home" \
-e ZILLOW__SINK__BROKER="localhost" \
-e ZILLOW__SINK__DISCOVERYENABLED="true" \
mannkind/zillow2mqtt:latest
```

OR

```bash
ZILLOW__SOURCE__APIKEY="B1-AWz18xy032zklA_6Nmn1" \
ZILLOW__SHARED__RESOURCES__0__ZPID="69103754" \
ZILLOW__SHARED__RESOURCES__0__Slug="home" \
ZILLOW__SINK__BROKER="localhost" \
ZILLOW__SINK__DISCOVERYENABLED="true" \
./zillow2mqtt 
```


## Configuration

Configuration happens via environmental variables

```bash
ZILLOW__SOURCE__APIKEY                     - The Zillow API key
ZILLOW__SOURCE__POLLINGINTERVAL            - [OPTIONAL] The delay between zestimates lookups, defaults to "1.00:03:31"
ZILLOW__SHARED__RESOURCES__#__ZPID         - The Zillow Property ID
ZILLOW__SHARED__RESOURCES__#__Slug         - The slug for the Zillow Property ID
ZILLOW__SINK__TOPICPREFIX                  - [OPTIONAL] The MQTT topic on which to publish the collection lookup results, defaults to "home/zillow"
ZILLOW__SINK__DISCOVERYENABLED             - [OPTIONAL] The MQTT discovery flag for Home Assistant, defaults to false
ZILLOW__SINK__DISCOVERYPREFIX              - [OPTIONAL] The MQTT discovery prefix for Home Assistant, defaults to "homeassistant"
ZILLOW__SINK__DISCOVERYNAME                - [OPTIONAL] The MQTT discovery name for Home Assistant, defaults to "zillow"
ZILLOW__SINK__BROKER                       - [OPTIONAL] The MQTT broker, defaults to "test.mosquitto.org"
ZILLOW__SINK__USERNAME                     - [OPTIONAL] The MQTT username, default to ""
ZILLOW__SINK__PASSWORD                     - [OPTIONAL] The MQTT password, default to ""
```
