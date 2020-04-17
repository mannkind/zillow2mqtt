using System;

namespace Zillow.Models.SourceManager
{
    /// <summary>
    /// The source options
    /// </summary>
    public class Opts
    {
        public const string Section = "Zillow:Source";

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeSpan PollingInterval { get; set; } = new TimeSpan(24, 3, 31);
    }
}
