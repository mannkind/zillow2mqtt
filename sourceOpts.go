package main

import "time"

type sourceOpts struct {
	globalOpts
	APIKey         string        `env:"ZILLOW_APIKEY,required"`
	LookupInterval time.Duration `env:"ZILLOW_LOOKUPINTERVAL" envDefault:"24h"`
}
