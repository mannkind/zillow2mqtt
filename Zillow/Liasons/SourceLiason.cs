using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Interfaces;
using TwoMQTT.Liasons;
using Zillow.DataAccess;
using Zillow.Models.Options;
using Zillow.Models.Shared;
using Zillow.Models.Source;

namespace Zillow.Liasons
{
    /// <summary>
    /// A class representing a managed way to interact with a source.
    /// </summary>
    public class SourceLiason : SourceLiasonBase<Resource, object, SlugMapping, ISourceDAO, SharedOpts>, ISourceLiason<Resource, object>
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
            return result switch
            {
                Response => new Resource
                {
                    ZPID = result.ZPID,
                    ZEstimate = result.Amount,
                },
                _ => null,
            };
        }
    }
}
