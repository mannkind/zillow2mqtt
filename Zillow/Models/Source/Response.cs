using System;

namespace Zillow.Models.Source;

/// <summary>
/// The response from the source
/// </summary>
public record Response
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
    public string Amount { get; init; } = string.Empty;
}
