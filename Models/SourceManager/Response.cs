using System;

namespace Zillow.Models.SourceManager
{
    /// <summary>
    /// The response from the source
    /// </summary>
    public class Response
    {
        public string ZPID { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0.0M;

        public override string ToString() => $"ZPID: {this.ZPID}, Amount: {this.Amount}";
    }
}