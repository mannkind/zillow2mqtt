package main

type eventData struct {
	Zpid   string `mqtt:",ignore" mqttDiscoveryType:",ignore"`
	Amount int    `mqtt:"zestimate" mqttDiscoveryType:"sensor"`
}

type event struct {
	version int64
	data    eventData
}

type observer interface {
	receiveState(event)
	receiveCommand(int64, event)
}

type publisher interface {
	register(observer)
}
