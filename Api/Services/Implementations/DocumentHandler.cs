using Api.Services.Contracts;
using iText.Forms;
using iText.Kernel.Pdf;

namespace Api.Services.Implementations;

public class DocumentHandler : IHandler
{
    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <inheritdoc />
    public void Dispose() { }

    /// <inheritdoc />
    public byte[] Template { get; set; } = Array.Empty<byte>();

    /// <inheritdoc />
    public Stream Generate(IDictionary<string, string> properties)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<ICollection<string>> Fields()
    {
        await using var template = new MemoryStream(Template);
        var memory = new MemoryStream();
        await using var writer = new PdfWriter(memory);
        using var document = new PdfDocument(new PdfReader(template), writer);
        var form = PdfAcroForm.GetAcroForm(document, false);
        var fields = form.GetAllFormFields().Select(f => f.Key);
        document.Close();
        return fields.ToList();
    }

    /// <inheritdoc />
    public async Task<Stream> GenerateAsync(IDictionary<string, string> properties)
    {
        await using var template = new MemoryStream(Template);
        var memory = new MemoryStream();
        await using var writer = new PdfWriter(memory);
        using var document = new PdfDocument(new PdfReader(template), writer);
        writer.SetCloseStream(false);   // we do not want to close the memory stream
        var form = PdfAcroForm.GetAcroForm(document, false);
        foreach (var prop in properties)
            form.GetField(prop.Key)?.SetValue(prop.Value);
        document.Close();
        memory.Position = 0;    // we reset the position
        return memory;
    }
}