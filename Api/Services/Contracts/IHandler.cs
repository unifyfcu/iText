namespace Api.Services.Contracts;

/// <summary>
/// Handler for PDF generation
/// <see cref="IAsyncDisposable" />
/// <see cref="IDisposable" />
/// </summary>
public interface IHandler : IAsyncDisposable, IDisposable
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
    /// <returns>An open stream that contains a duplicate PDF from the given template and the provided values</returns>
    Stream Generate(IDictionary<string, string> properties);
    /// <summary>
    /// Asynchronous version
    /// <see cref="Generate(IDictionary{string,string})" />
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    Task<Stream> GenerateAsync(IDictionary<string, string> properties);
}