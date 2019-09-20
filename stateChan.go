package main

type stateChannel = chan zestimate

func newStateChannel() stateChannel {
	return make(stateChannel, 100)
}
