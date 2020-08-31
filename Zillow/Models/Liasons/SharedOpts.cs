using System.Collections.Generic;
using TwoMQTT.Core.Interfaces;
using Zillow.Models.Shared;

namespace Zillow.Models.Options
{
    /// <summary>
    /// The shared options across the application
    /// </summary>
    public class SharedOpts : ISharedOpts<SlugMapping>
    {
        public const string Section = "Zillow";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="SlugMapping"></typeparam>
        /// <returns></returns>
        public List<SlugMapping> Resources { get; set; } = new List<SlugMapping>();
    }
}
