package main

type globalOpts struct {
	ZPIDS sourceMapping `env:"ZILLOW_ZPIDS" envDefault:""`
}
