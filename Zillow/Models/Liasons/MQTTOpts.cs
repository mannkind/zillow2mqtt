using TwoMQTT.Core.Models;

namespace Zillow.Models.Options
{
    /// <summary>
    /// The sink options
    /// </summary>
    public class MQTTOpts : MQTTManagerOptions
    {
        public const string Section = "Zillow:MQTT";
        public const string TopicPrefixDefault = "home/zillow";
        public const string DiscoveryNameDefault = "zillow";
    }
}
