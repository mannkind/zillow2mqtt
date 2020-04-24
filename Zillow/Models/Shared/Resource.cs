using System;

namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared resource across the application
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ZPID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public decimal ZEstimate { get; set; } = 0.0M;
    }
}
