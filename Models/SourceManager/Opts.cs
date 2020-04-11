using System;

namespace Zillow.Models.SourceManager
{
    /// <summary>
    /// The source options
    /// </summary>
    public class Opts
    {
        public const string Section = "Zillow:Source";

        public string ApiKey { get; set; } = string.Empty;
        public TimeSpan PollingInterval { get; set; } = new TimeSpan(24, 3, 31);

        public override string ToString() => $"Polling Interval: {this.PollingInterval}";
    }
}
