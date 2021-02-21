using System;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = Section + ":" + nameof(ApiKey) + " is missing")]
        public string ApiKey { get; init; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = Section + ":" + nameof(PollingInterval) + " is missing")]
        public TimeSpan PollingInterval { get; init; } = new(24, 3, 31);
    }
}
