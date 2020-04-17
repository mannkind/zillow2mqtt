using System;

namespace Zillow.Models.SourceManager
{
    /// <summary>
    /// The response from the source
    /// </summary>
    public class FetchResponse
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
        public decimal Amount { get; set; } = Decimal.MinValue;

        /// <inheritdoc />
        public override string ToString() => $"ZPID: {this.ZPID}, Amount: {this.Amount}";
    }
}