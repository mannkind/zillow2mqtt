using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TwoMQTT.Core.DataAccess;
using Zillow.Models.Shared;
using Zillow.Services;

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
            this.ZillowClient = new ZillowClient(opts.Value.ApiKey);
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
        /// The Client to access the source.
        /// </summary>
        private readonly ZillowClient ZillowClient;

        /// <summary>
        /// Fetch one response from the source
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Models.SourceManager.FetchResponse?> FetchAsync(string zpid,
            CancellationToken cancellationToken = default)
        {
            this.Logger.LogDebug($"Started finding {zpid} from Zillow");
            var result = await this.ZillowClient.GetZestimateAsync(zpid);
            if (result == null)
            {
                this.Logger.LogDebug($"Unable to find {zpid} from Zillow");
                return null;
            }

            this.Logger.LogDebug($"Finished finding {zpid} from Zillow");

            return new Models.SourceManager.FetchResponse
            {
                ZPID = zpid,
                Amount = result.response.zestimate.amount.Value,
            };
        }
    }
}
