using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwoMQTT.Core.Interfaces;
using Zillow.Models.Shared;
using Zillow.Models.Source;
using Zillow.Services;

namespace Zillow.DataAccess
{
    public interface ISourceDAO : ISourceDAO<SlugMapping, Response, object, object>
    {
    }

    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    public class SourceDAO : ISourceDAO
    {
        /// <summary>
        /// Initializes a new instance of the SourceDAO class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="zillowClient"></param>
        /// <returns></returns>
        public SourceDAO(ILogger<SourceDAO> logger, IZillowClient zillowClient)
        {
            this.Logger = logger;
            this.ZillowClient = zillowClient;
        }

        /// <inheritdoc />
        public async Task<Response?> FetchOneAsync(SlugMapping data,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.FetchAsync(data.ZPID, cancellationToken);
            }
            catch (Exception e)
            {
                var msg = e switch
                {
                    HttpRequestException => "Unable to fetch from the Zillow API",
                    JsonException => "Unable to deserialize response from the Zillow API",
                    _ => "Unable to send to the Zillow API"
                };
                this.Logger.LogError(msg, e);
                return null;
            }
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        private readonly ILogger<SourceDAO> Logger;

        /// <summary>
        /// The client to access the source.
        /// </summary>
        private readonly IZillowClient ZillowClient;

        /// <summary>
        /// Fetch one response from the source
        /// </summary>
        /// <param name="zpid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Response?> FetchAsync(string zpid,
            CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Started finding {zpid} from Zillow", zpid);
            var result = await this.ZillowClient.GetZestimateAsync(zpid);
            this.Logger.LogDebug("Finished finding {zpid} from Zillow", zpid);

            return result switch
            {
                Zillow.Services.Schema.zestimateResultType => new Response
                {
                    ZPID = zpid,
                    Amount = result.response.zestimate.amount.Value,
                },
                _ => null
            };
        }
    }
}
