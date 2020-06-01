using System.Linq;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Core.DataAccess;
using TwoMQTT.Core.Managers;
using Zillow.Models.Shared;
using Zillow.Models.SourceManager;

namespace Zillow.Managers
{
    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    public class SourceManager : APIPollingManager<SlugMapping, FetchResponse, object, Resource, Command>
    {
        /// <summary>
        /// Initializes a new instance of the SourceManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sharedOpts"></param>
        /// <param name="opts"></param>
        /// <param name="outgoingData"></param>
        /// <param name="incomingCommand"></param>
        /// <param name="sourceDAO"></param>
        /// <returns></returns>
        public SourceManager(ILogger<SourceManager> logger, IOptions<Models.Shared.Opts> sharedOpts,
            IOptions<Models.SourceManager.Opts> opts, ChannelWriter<Resource> outgoingData,
            ChannelReader<Command> incomingCommand, ISourceDAO<SlugMapping, Command, FetchResponse, object> sourceDAO) :
            base(logger, outgoingData, incomingCommand, sharedOpts.Value.Resources, opts.Value.PollingInterval,
                sourceDAO, SourceSettings(sharedOpts.Value, opts.Value))
        {
        }

        /// <inheritdoc />
        protected override Resource MapResponse(FetchResponse src) =>
            new Resource
            {
                ZPID = src.ZPID,
                ZEstimate = src.Amount,
            };

        private static string SourceSettings(Models.Shared.Opts sharedOpts, Models.SourceManager.Opts opts) =>
            $"ApiKey: {opts.ApiKey}\n" +
            $"PollingInterval: {opts.PollingInterval}\n" +
            $"Resources: {string.Join(",", sharedOpts.Resources.Select(x => $"{x.ZPID}:{x.Slug}"))}\n" +
            $"";
    }
}
