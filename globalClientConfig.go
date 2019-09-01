package main

type globalClientConfig struct {
	ZPIDS zpidMapping `env:"ZILLOW_ZPIDS" envDefault:""`
}
