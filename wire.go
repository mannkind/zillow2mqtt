//+build wireinject

package main

import (
	"github.com/google/wire"
	"github.com/mannkind/twomqtt"
	"github.com/mannkind/zillow2mqtt/mqtt"
	"github.com/mannkind/zillow2mqtt/shared"
	"github.com/mannkind/zillow2mqtt/source"
)

func initialize() *app {
	wire.Build(
		newApp,
		shared.NewOpts,
		shared.NewRepresentationChannel,
		shared.NewRepresentationChannelIncoming,
		shared.NewRepresentationChannelOutgoing,
		mqtt.NewOpts,
		mqtt.NewWriter,
		source.NewOpts,
		source.NewService,
		source.NewReader,
		wire.FieldsOf(new(mqtt.Opts), "MQTTOpts"),
		twomqtt.NewMQTT,
	)

	return &app{}
}
