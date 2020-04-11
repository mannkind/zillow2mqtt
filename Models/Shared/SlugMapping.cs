namespace Zillow.Models.Shared
{
    /// <summary>
    /// The shared key info => slug mapping across the application
    /// </summary>
    public class SlugMapping
    {
        public string ZPID { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public override string ToString() => $"ZPID: {this.ZPID}, Slug: {this.Slug}";
    }
}
