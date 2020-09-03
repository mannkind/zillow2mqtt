using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Core;
using TwoMQTT.Core.Models;
using TwoMQTT.Core.Utils;
using TwoMQTT.Core.Liasons;
using Zillow.Models.Options;
using Zillow.Models.Shared;
using TwoMQTT.Core.Interfaces;

namespace Zillow.Liasons
{
    /// <inheritdoc />
    public class MQTTLiason : MQTTLiasonBase<Resource, object, SlugMapping, SharedOpts>, IMQTTLiason<Resource, object>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="generator"></param>
        /// <param name="sharedOpts"></param>
        public MQTTLiason(ILogger<MQTTLiason> logger, IMQTTGenerator generator, IOptions<SharedOpts> sharedOpts) :
            base(logger, generator, sharedOpts)
        {
        }

        /// <inheritdoc />
        public IEnumerable<(string topic, string payload)> MapData(Resource input)
        {
            var results = new List<(string, string)>();
            var slug = this.Questions
                .Where(x => x.ZPID == input.ZPID)
                .Select(x => x.Slug)
                .FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(slug))
            {
                this.Logger.LogDebug("Unable to find slug for {zpid}", input.ZPID);
                return results;
            }

            this.Logger.LogDebug("Found slug {slug} for incoming data for {zpid}", slug, input.ZPID);
            results.AddRange(new[]
                {
                    (this.Generator.StateTopic(slug, nameof(Resource.ZEstimate)), input.ZEstimate.ToString()),
                }
            );

            return results;
        }

        /// <inheritdoc />
        public IEnumerable<(string slug, string sensor, string type, MQTTDiscovery discovery)> Discoveries()
        {
            var discoveries = new List<(string, string, string, MQTTDiscovery)>();
            var assembly = Assembly.GetAssembly(typeof(Program))?.GetName() ?? new AssemblyName();
            var mapping = new[]
            {
                new { Sensor = nameof(Resource.ZEstimate), Type = Const.SENSOR },
            };

            foreach (var input in this.Questions)
            {
                foreach (var map in mapping)
                {
                    this.Logger.LogDebug("Generating discovery for {zpid} - {sensor}", input.ZPID, map.Sensor);
                    var discovery = this.Generator.BuildDiscovery(input.Slug, map.Sensor, assembly, false);
                    discovery = discovery with { Icon = "mdi:home-variant" };

                    discoveries.Add((input.Slug, map.Sensor, map.Type, discovery));
                }
            }

            return discoveries;
        }
    }
}