package shared

// Representation is a data structure for inter-application communication
type Representation struct {
	Zpid   string `mqtt:",ignore" mqttDiscoveryType:",ignore"`
	Amount int    `mqtt:"zestimate" mqttDiscoveryType:"sensor"`
}
