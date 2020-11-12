namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared resource across the application
    /// </summary>
    public record Resource
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
        public string ZEstimate { get; init; } = string.Empty;

        /// <inheritdoc />
        public override string ToString() => $"ZPID: {this.ZPID}; ZEstimate: {this.ZEstimate}";
    }
}
