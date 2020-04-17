using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using Zillow.Models.Shared;
using System.Xml;
using Newtonsoft.Json.Linq;
using System;
using TwoMQTT.Core.DataAccess;

namespace Zillow.DataAccess
{
    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    public class SourceDAO : HTTPSourceDAO<SlugMapping, Command, Models.SourceManager.FetchResponse, object>
    {
        /// <summary>
        /// Initializes a new instance of the SourceDAO class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="opts"></param>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public SourceDAO(ILogger<SourceDAO> logger, IOptions<Models.SourceManager.Opts> opts, 
            IHttpClientFactory httpClientFactory) : 
            base(logger, httpClientFactory)
        {
            this.ApiKey = opts.Value.ApiKey;
        }

        /// <inheritdoc />
        public override async Task<Models.SourceManager.FetchResponse?> FetchOneAsync(SlugMapping data, 
            CancellationToken cancellationToken = default)
        {
            try 
            {
                return await this.FetchAsync(data.ZPID, cancellationToken);
            }
            catch (Exception e)
            {
                var msg = e is HttpRequestException ? "Unable to fetch from the Zillow API" :
                          e is JsonException ? "Unable to deserialize response from the Zillow API" :
                          "Unable to send to the Zillow API";
                this.Logger.LogError(msg, e);
                return null;
            }
        }

        /// <summary>
        /// The API Key to access the source.
        /// </summary>
        private readonly string ApiKey;

        /// <summary>
        /// Fetch one response from the source
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Models.SourceManager.FetchResponse?> FetchAsync(string zpid, 
            CancellationToken cancellationToken = default)
        {
            var baseUrl = "https://www.zillow.com/webservice/GetZestimate.htm";
            var query = $"zws-id={this.ApiKey}&zpid={zpid}";
            var resp = await this.Client.GetAsync($"{baseUrl}?{query}", cancellationToken);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            var jsonObj = JObject.Parse(jsonText);
            var response = jsonObj.SelectToken("$.Zestimate:zestimate.response") ?? new JObject();
            return new Models.SourceManager.FetchResponse
            {
                ZPID = zpid,
                Amount = response.SelectToken(".zestimate.amount")?.Value<decimal>("#text") ?? default,
            };
        }
    }
}
