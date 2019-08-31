package main

type zpidMapping = map[string]string

type zestimate struct {
	Zpid   string `mqtt:",ignore" mqttDiscoveryType:",ignore"`
	Amount int    `mqtt:"zestimate" mqttDiscoveryType:"sensor"`
}
