using TwoMQTT.Core.Models;

namespace Zillow.Models.SinkManager
{
    /// <summary>
    /// The sink options
    /// </summary>
    public class Opts : MQTTManagerOptions
    {
        public const string Section = "Zillow:MQTT";

        /// <summary>
        /// 
        /// </summary>
        public Opts()
        {
            this.TopicPrefix = "home/zillow";
            this.DiscoveryName = "zillow";
        }
    }
}
