namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared key info => slug mapping across the application
    /// </summary>
    public record SlugMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ZPID { get; init; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Slug { get; init; } = string.Empty;
    }
}
