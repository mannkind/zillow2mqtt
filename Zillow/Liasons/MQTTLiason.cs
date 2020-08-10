using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Core;
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Models;
using TwoMQTT.Core.Utils;
using Zillow.Models.Options;
using Zillow.Models.Shared;

namespace Zillow.Liasons
{
    /// <inheritdoc />
    public class MQTTLiason : IMQTTLiason<Resource, Command>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="generator"></param>
        /// <param name="sharedOpts"></param>
        public MQTTLiason(ILogger<MQTTLiason> logger, IMQTTGenerator generator, IOptions<SharedOpts> sharedOpts)
        {
            this.Logger = logger;
            this.Generator = generator;
            this.Questions = sharedOpts.Value.Resources;
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
                    discovery.Icon = "mdi:home-variant";

                    discoveries.Add((input.Slug, map.Sensor, map.Type, discovery));
                }
            }

            return discoveries;
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        private readonly ILogger<MQTTLiason> Logger;

        /// <summary>
        /// The questions to ask the source (typically some kind of key/slug pairing).
        /// </summary>
        private readonly List<SlugMapping> Questions;

        /// <summary>
        /// The MQTT generator used for things such as availability topic, state topic, command topic, etc.
        /// </summary>
        private readonly IMQTTGenerator Generator;
    }
}