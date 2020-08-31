using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Liasons;
using Zillow.DataAccess;
using Zillow.Models.Options;
using Zillow.Models.Shared;
using Zillow.Models.Source;

namespace Zillow.Liasons
{
    /// <summary>
    /// A class representing a managed way to interact with a source.
    /// </summary>
    public class SourceLiason : SourceLiasonBase<Resource, Command, SlugMapping, ISourceDAO, SharedOpts>, ISourceLiason<Resource, Command>
    {
        public SourceLiason(ILogger<SourceLiason> logger, ISourceDAO sourceDAO,
            IOptions<SourceOpts> opts, IOptions<SharedOpts> sharedOpts) :
            base(logger, sourceDAO, sharedOpts)
        {
            this.Logger.LogInformation(
                "ApiKey: {apiKey}\n" +
                "PollingInterval: {pollingInterval}\n" +
                "Resources: {@resources}\n" +
                "",
                opts.Value.ApiKey,
                opts.Value.PollingInterval,
                sharedOpts.Value.Resources
            );
        }

        /// <inheritdoc />
        protected override async Task<Resource?> FetchOneAsync(SlugMapping key, CancellationToken cancellationToken)
        {
            var result = await this.SourceDAO.FetchOneAsync(key, cancellationToken);
            var resp = result != null ? this.MapData(result) : null;
            return resp;
        }

        /// <summary>
        /// Map the source response to a shared response representation.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Resource MapData(Response src) =>
            new Resource
            {
                ZPID = src.ZPID,
                ZEstimate = src.Amount,
            };
    }
}
