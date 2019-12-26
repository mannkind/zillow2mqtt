package main

type sourceRep struct {
	Zpid   string `mqtt:",ignore" mqttDiscoveryType:",ignore"`
	Amount int    `mqtt:"zestimate" mqttDiscoveryType:"sensor"`
}
