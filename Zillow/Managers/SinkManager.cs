using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Extensions.ManagedClient;
using TwoMQTT.Core;
using TwoMQTT.Core.Managers;
using TwoMQTT.Core.Models;
using Zillow.Models.Shared;

namespace Zillow.Managers
{
    /// <summary>
    /// An class representing a managed way to interact with a sink.
    /// </summary>
    public class SinkManager : MQTTManager<SlugMapping, Resource, Command>
    {
        /// <summary>
        /// Initializes a new instance of the SinkManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sharedOpts"></param>
        /// <param name="opts"></param>
        /// <param name="incomingData"></param>
        /// <param name="outgoingCommand"></param>
        /// <returns></returns>
        public SinkManager(ILogger<SinkManager> logger, IOptions<Opts> sharedOpts, IOptions<Models.SinkManager.Opts> opts,
            IManagedMqttClient client, ChannelReader<Resource> incomingData, ChannelWriter<Command> outgoingCommand) :
            base(logger, opts, client, incomingData, outgoingCommand, sharedOpts.Value.Resources, string.Empty)
        {
        }

        /// <inheritdoc />
        protected override async Task HandleIncomingDataAsync(Resource input,
            CancellationToken cancellationToken = default)
        {
            var slug = this.Questions
                .Where(x => x.ZPID == input.ZPID)
                .Select(x => x.Slug)
                .FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(slug))
            {
                this.Logger.LogDebug($"Unable to find slug for {input.ZPID}");
                return;
            }

            this.Logger.LogDebug($"Found slug {slug} for incoming data for {input.ZPID}");
            this.Logger.LogDebug($"Started publishing data for slug {slug}");
            var publish = new[]
            {
                (this.StateTopic(slug, nameof(Resource.ZEstimate)), input.ZEstimate.ToString()),
            };
            await this.PublishAsync(publish, cancellationToken);
            this.Logger.LogDebug($"Finished publishing data for slug {slug}");
        }

        /// <inheritdoc />
        protected override IEnumerable<(string slug, string sensor, string type, MQTTDiscovery discovery)> Discoveries()
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
                    this.Logger.LogDebug($"Generating discovery for {input.ZPID} - {map.Sensor}");
                    var discovery = this.BuildDiscovery(input.Slug, map.Sensor, assembly, false);
                    discovery.Icon = "mdi:home-variant";

                    discoveries.Add((input.Slug, map.Sensor, map.Type, discovery));
                }
            }

            return discoveries;
        }
    }
}