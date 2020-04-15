using TwoMQTT.Core.Models;

namespace Zillow.Models.SinkManager
{
    /// <summary>
    /// The sink options
    /// </summary>
    public class Opts : MQTTManagerOptions
    {
        public const string Section = "Zillow:Sink";

        public Opts()
        {
            this.TopicPrefix = "home/zillow";
            this.DiscoveryName = "zillow";
        }
    }
}
