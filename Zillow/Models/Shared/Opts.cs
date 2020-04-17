using System.Collections.Generic;

namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared options across the application
    /// </summary>
    public class Opts
    {
        public const string Section = "Zillow:Shared";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="SlugMapping"></typeparam>
        /// <returns></returns>
        public List<SlugMapping> Resources { get; set; } = new List<SlugMapping>();
    }
}
