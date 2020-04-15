using System;

namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared resource across the application
    /// </summary>
    public class Resource
    {
        public string ZPID { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0.0M;

        public override string ToString() => $"ZPID: {this.ZPID}, Amount: {this.Amount}";

        public static Resource From(SourceManager.Response obj) => 
            new Resource
            {
                ZPID = obj.ZPID,
                Amount = obj.Amount,
            };
    }
}
