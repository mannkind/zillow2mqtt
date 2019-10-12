module github.com/mannkind/zillow2mqtt

go 1.13

require (
	github.com/caarlos0/env/v6 v6.0.0
	github.com/eclipse/paho.mqtt.golang v1.2.0
	github.com/google/wire v0.3.0
	github.com/jmank88/zillow v0.0.0-20170915145436-a40f07eca489
	github.com/kr/pretty v0.1.0 // indirect
	github.com/magefile/mage v1.9.0
	github.com/mannkind/twomqtt v0.3.3
	github.com/robfig/cron/v3 v3.0.0
	github.com/sirupsen/logrus v1.4.2
)

// local development
// replace github.com/mannkind/twomqtt => ../twomqtt
