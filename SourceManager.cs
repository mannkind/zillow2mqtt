using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using TwoMQTT.Core.Communication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Zillow.Models.Shared;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Zillow
{
    public class SourceManager : HTTPManager<SlugMapping, Resource, Command>
    {
        public SourceManager(ILogger<SourceManager> logger, IOptions<Opts> sharedOpts, IOptions<Models.SourceManager.Opts> opts, ChannelWriter<Resource> outgoing, ChannelReader<Command> incoming, IHttpClientFactory httpClientFactory) :
            base(logger, outgoing, incoming, httpClientFactory.CreateClient())
        {
            this.opts = opts.Value;
            this.sharedOpts = sharedOpts.Value;
        }
        protected readonly Models.SourceManager.Opts opts;
        protected readonly Opts sharedOpts;

        /// <inheritdoc />
        protected override void LogSettings()
        {
            var resources = string.Join(",",
                this.sharedOpts.Resources.Select(x => $"{x.ZPID}:{x.Slug}")
            );

            this.logger.LogInformation(
                $"ApiKey:                {this.opts.ApiKey}\n" +
                $"PollingInterval:       {this.opts.PollingInterval}\n" +
                $"Resources:             {resources}\n" +
                $""
            );
        }

        /// <inheritdoc />
        protected override async Task PollAsync(CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Polling");

            var tasks = new List<Task<Models.SourceManager.Response>>();
            foreach (var key in this.sharedOpts.Resources)
            {
                this.logger.LogInformation($"Looking up {key}");
                tasks.Add(this.FetchOneAsync(key, cancellationToken));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                this.logger.LogInformation($"Found {result}");
                await this.outgoing.WriteAsync(Resource.From(result), cancellationToken);
            }
        }

        /// <inheritdoc />
        protected override Task DelayAsync(CancellationToken cancellationToken = default) => 
            Task.Delay(this.opts.PollingInterval, cancellationToken);

        /// <summary>
        /// Fetch one record from the source
        /// </summary>
        private async Task<Models.SourceManager.Response> FetchOneAsync(SlugMapping key, CancellationToken cancellationToken = default)
        {
            var baseUrl = "https://www.zillow.com/webservice/GetZestimate.htm";
            var query = $"zws-id={this.opts.ApiKey}&zpid={key.ZPID}";
            var resp = await this.client.GetAsync($"{baseUrl}?{query}", cancellationToken);
            var content = await resp.Content.ReadAsStringAsync();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            var jsonObj = JObject.Parse(jsonText);
            var response = jsonObj.SelectToken("$.Zestimate:zestimate.response") ?? new JObject();
            return new Models.SourceManager.Response 
            {
                ZPID = key.ZPID,
                Amount = response.SelectToken(".zestimate.amount")?.Value<decimal>("#text") ?? default,
            };
        }
    }
}
