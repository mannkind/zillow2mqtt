using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using TwoMQTT.Core.Communication;
using Zillow.Models.Shared;

namespace Zillow
{
    public class SinkManager : MQTTManager<Resource, Command>
    {
        public SinkManager(ILogger<SinkManager> logger, IOptions<Opts> sharedOpts, IOptions<Models.SinkManager.Opts> opts, ChannelReader<Resource> inputChannel, ChannelWriter<Command> outputChannel) :
            base(logger, opts, inputChannel, outputChannel)
        {
            this.sharedOpts = sharedOpts.Value;
        }
        protected readonly Opts sharedOpts;

        /// <inheritdoc />
        protected override async Task HandleIncomingAsync(Resource input, CancellationToken cancellationToken = default)
        {
            var slug = this.sharedOpts.Resources
                .Where(x => x.ZPID == input.ZPID)
                .Select(x => x.Slug)
                .FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(slug))
            {
                return;
            }

            await Task.WhenAll(
                this.PublishAsync(this.StateTopic(slug, "amount"), input.Amount.ToString())
            );

        }

        /// <inheritdoc />
        protected override async Task HandleDiscoveryAsync(CancellationToken cancellationToken = default)
        {
            if (!this.opts.DiscoveryEnabled)
            {
                return;
            }

            var tasks = new List<Task>();
            var assembly = Assembly.GetAssembly(typeof(Program))?.GetName() ?? new AssemblyName();
            var mapping = new [] 
            {
                new { Sensor = nameof(Resource.Amount).ToLower(), Type = "sensor" },
            };

            foreach (var input in this.sharedOpts.Resources)
            {
                foreach (var map in mapping) 
                {
                    var discovery = this.BuildDiscovery(input.Slug, map.Sensor, assembly, false);
                    tasks.Add(this.PublishDiscoveryAsync(input.Slug, map.Sensor, map.Type, discovery, cancellationToken));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default) 
        {
            if (this.knownMessages.ContainsKey(topic) && this.knownMessages[topic] == payload)
            {
                this.logger.LogDebug($"Duplicate '{payload}' found on '{topic}'");
                return;
            }

            this.logger.LogInformation($"Publishing '{payload}' on '{topic}'");
            await this.client.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                cancellationToken
            );

            this.knownMessages[topic] = payload;
        }
    }
}