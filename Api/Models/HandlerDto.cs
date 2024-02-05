using System.Text.Json.Serialization;

namespace Api.Models;

/// <summary>
/// The handler information
/// </summary>
public class HandlerDto
{
    /// <summary>
    /// The template URI; must be a PDF location
    /// </summary>
    public string? TemplateUrl { get; set; }

    /// <summary>
    /// Template Byte array stored as a base64 string
    /// </summary>
    public string? TemplateData { get; set; }

    /// <summary>
    /// Template Data
    /// </summary>
    [JsonIgnore]
    public byte[] Template =>
        // we need to first determine if we have a template url or a base64 encoded byte array
        string.IsNullOrEmpty(TemplateUrl) ? Convert.FromBase64String(TemplateData!) : Array.Empty<byte>();

    // TODO - actually download the template pdf
    /// <summary>
    /// Key / Value pair of ACRO form field names and the associated values tied to it
    /// </summary>
    public IEnumerable<IDictionary<string, string>> Properties { get; set; } = new List<IDictionary<string, string>>();
}