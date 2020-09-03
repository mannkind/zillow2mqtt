using System;

namespace Zillow.Models.Options
{
    /// <summary>
    /// The source options
    /// </summary>
    public record SourceOpts
    {
        public const string Section = "Zillow";

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ApiKey { get; init; } = "B1-AWz18xy032zklA_6Nmn1";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeSpan PollingInterval { get; init; } = new TimeSpan(24, 3, 31);
    }
}
