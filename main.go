package main

import (
	"os"

	"github.com/mannkind/zillow2mqtt/shared"
	log "github.com/sirupsen/logrus"
)

func init() {
	log.SetOutput(os.Stdout)
	log.SetLevel(log.InfoLevel)
}

func main() {
	log.Infof("%s version: %s", shared.Name, shared.Version)

	x := initialize()
	x.run()

	select {}
}
