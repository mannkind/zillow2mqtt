# zillow2mqtt

[![Software
License](https://img.shields.io/badge/License-MIT-orange.svg?style=flat-square)](https://github.com/mannkind/zillow2mqtt/blob/main/LICENSE.md)
[![Build Status](https://github.com/mannkind/zillow2mqtt/workflows/Main%20Workflow/badge.svg)](https://github.com/mannkind/zillow2mqtt/actions)
[![Coverage Status](https://img.shields.io/codecov/c/github/mannkind/zillow2mqtt/main.svg)](http://codecov.io/github/mannkind/zillow2mqtt?branch=main)

An experiment to publish Zillow ZEstimates to MQTT.

See also Zillow's API documentation at <http://www.zillow.com/howto/api/APIOverview.htm>.

## Use

The application can be locally built using `dotnet build` or you can utilize the multi-architecture Docker image(s).

### Example

```bash
docker run \
-e ZILLOW__APIKEY="B1-AWz18xy032zklA_6Nmn1" \
-e ZILLOW__RESOURCES__0__ZPID="69103754" \
-e ZILLOW__RESOURCES__0__Slug="home" \
-e ZILLOW__MQTT__BROKER="localhost" \
-e ZILLOW__MQTT__DISCOVERYENABLED="true" \
mannkind/zillow2mqtt:latest
```

OR

```bash
ZILLOW__APIKEY="B1-AWz18xy032zklA_6Nmn1" \
ZILLOW__RESOURCES__0__ZPID="69103754" \
ZILLOW__RESOURCES__0__Slug="home" \
ZILLOW__MQTT__BROKER="localhost" \
ZILLOW__MQTT__DISCOVERYENABLED="true" \
./zillow2mqtt 
```


## Configuration

Configuration happens via environmental variables

```bash
ZILLOW__APIKEY                             - The Zillow API key
ZILLOW__POLLINGINTERVAL                    - [OPTIONAL] The delay between zestimates lookups, defaults to "1.00:03:31"
ZILLOW__RESOURCES__#__ZPID                 - The n-th iteration of a Zillow Property ID for a specific property
ZILLOW__RESOURCES__#__Slug                 - The n-th iteration of a slug to identify the specific Zillow Property ID
ZILLOW__MQTT__TOPICPREFIX                  - [OPTIONAL] The MQTT topic on which to publish the collection lookup results, defaults to "home/zillow"
ZILLOW__MQTT__DISCOVERYENABLED             - [OPTIONAL] The MQTT discovery flag for Home Assistant, defaults to false
ZILLOW__MQTT__DISCOVERYPREFIX              - [OPTIONAL] The MQTT discovery prefix for Home Assistant, defaults to "homeassistant"
ZILLOW__MQTT__DISCOVERYNAME                - [OPTIONAL] The MQTT discovery name for Home Assistant, defaults to "zillow"
ZILLOW__MQTT__BROKER                       - [OPTIONAL] The MQTT broker, defaults to "test.mosquitto.org"
ZILLOW__MQTT__USERNAME                     - [OPTIONAL] The MQTT username, default to ""
ZILLOW__MQTT__PASSWORD                     - [OPTIONAL] The MQTT password, default to ""
```

## Prior Implementations

### Golang
* Last Commit: [682c80313cee963bd1c6c0948577ebffd9d551d2](https://github.com/mannkind/zillow2mqtt/commit/682c80313cee963bd1c6c0948577ebffd9d551d2)
* Last Docker Image: [mannkind/zillow2mqtt:v0.4.20061.0152](https://hub.docker.com/layers/mannkind/zillow2mqtt/v0.4.20061.0152/images/sha256-4c450faf8bbac5a6dd55fdb084cebdeae256c01a9b27580b9f0302ec98e6842c?context=explore)