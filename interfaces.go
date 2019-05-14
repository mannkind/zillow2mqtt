package main

type eventData struct {
	Zpid   string
	Amount int
}

type event struct {
	version int64
	data    eventData
}

type observer interface {
	receive(event)
}

type publisher interface {
	register(observer)
	publish(event)
}
