using System.Threading.Channels;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zillow.Models.Shared;
using TwoMQTT.Core.Managers;
using Zillow.Models.SourceManager;
using TwoMQTT.Core.DataAccess;

namespace Zillow.Managers
{
    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    public class SourceManager : HTTPPollingManager<SlugMapping, FetchResponse, object, Resource, Command>
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
            ChannelReader<Command> incomingCommand,
            IHTTPSourceDAO<SlugMapping, Command, FetchResponse, object> sourceDAO) :
            base(logger, outgoingData, incomingCommand, sharedOpts.Value.Resources, opts.Value.PollingInterval, sourceDAO)
        {
            this.Opts = opts.Value;
            this.SharedOpts = sharedOpts.Value;
        }

        /// <inheritdoc />
        protected override void LogSettings() =>
            this.Logger.LogInformation(
                $"ApiKey: {this.Opts.ApiKey}\n" +
                $"PollingInterval: {this.Opts.PollingInterval}\n" +
                $"Resources: {this.SharedOpts.Resources.Select(x => $"{x.ZPID}:{x.Slug}")}\n" +
                $""
            );

        /// <inheritdoc />
        protected override Resource MapResponse(FetchResponse src) =>
            new Resource
            {
                ZPID = src.ZPID,
                ZEstimate = src.Amount,
            };

        /// <summary>
        /// The options for the source.
        /// </summary>
        private readonly Models.SourceManager.Opts Opts;

        /// <summary>
        /// The options that are shared.
        /// </summary>
        private readonly Models.Shared.Opts SharedOpts;
    }
}
