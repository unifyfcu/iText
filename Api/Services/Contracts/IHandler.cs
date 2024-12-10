namespace Api.Services.Contracts;

/// <summary>
/// Handler for PDF generation
/// </summary>
public interface IHandler
{
    /// <summary>
    /// Associated Form fields with the template
    /// </summary>
    /// <returns></returns>
    Task<ICollection<string>> Fields();
    /// <summary>
    /// PDF Template
    /// </summary>
    byte[] Template { get; set; }
    /// <summary>
    /// Attempts to generate a PDF document with the provided properties tied to it's Acro Form
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    Task<Stream> GenerateAsync(IDictionary<string, string> properties);
}