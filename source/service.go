package source

import (
	"github.com/jmank88/zillow"
	log "github.com/sirupsen/logrus"
)

// Service is for reading a directly from a source system
type Service struct {
	APIKey string
}

// NewService creates a new Service for reading a directly from a source system
func NewService() *Service {
	c := Service{}

	return &c
}

// SetAPIKey sets the required options to access the source system
func (c *Service) SetAPIKey(apikey string) {
	c.APIKey = apikey
}

// lookup data from the source system
func (c *Service) lookup(zpid string) (*zillow.ZestimateResult, error) {
	log.WithFields(log.Fields{
		"zpid": zpid,
	}).Info("Lookup zestimate")

	client := zillow.New(c.APIKey)

	result, err := client.GetZestimate(zillow.ZestimateRequest{Zpid: zpid})
	if err != nil {
		log.WithFields(log.Fields{
			"error": err,
			"zpid":  zpid,
		}).Error("Failed to look up zestimate")

		return nil, err
	}

	log.Debug("Finished zestimate lookup")
	return result, nil
}
