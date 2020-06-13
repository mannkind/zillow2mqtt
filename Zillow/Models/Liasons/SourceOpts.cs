using System;

namespace Zillow.Models.Options
{
    /// <summary>
    /// The source options
    /// </summary>
    public class SourceOpts
    {
        public const string Section = "Zillow";

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ApiKey { get; set; } = "B1-AWz18xy032zklA_6Nmn1";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeSpan PollingInterval { get; set; } = new TimeSpan(24, 3, 31);
    }
}
