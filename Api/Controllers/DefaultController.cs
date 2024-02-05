using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Api.Services.Contracts;
using RestSharp;

namespace Api.Controllers;

/// <summary>
/// General Bundling Controller
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class DefaultController(IHandler handler) : ControllerBase
{
    /// <summary>
    /// Get Fields
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("fields")]
    public async Task<IActionResult> GetFields([FromBody] HandlerDto handle)
    {
        await SetupDocument(handle);
        try
        {
            var fields = await handler.Fields();
            handler.Template = Array.Empty<byte>();
            return Ok(fields);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unable to fetch Form Fields");
            return Problem(e.Message);
        }
    }

    /// <summary>
    /// Get Single
    /// </summary>
    /// <param name="handle"><see cref="HandlerDto" /></param>
    /// <returns>A single PDF document</returns>
    /// <remarks>Attempts to return a single PDF document from the provided handle details</remarks>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> GetSingle([FromBody] HandlerDto handle)
    {
        Log.Information("Beginning Parse");
        if (handle.Properties.Count() > 1) return BadRequest("Can only parse a single file");
        var temp = Path.GetTempFileName();
        try
        {
            await SetupDocument(handle);
            Log.Debug($"Parsing Property Collection: {handle.Properties.Count()}");
            await using var stream = await handler.GenerateAsync(handle.Properties.First());
            await using (var file = new FileStream(temp, FileMode.Create))
                await stream.CopyToAsync(file);
            var f = new DeleteAfterReadingStream(temp, FileMode.Open, FileAccess.Read);
            Log.Information($"Created PDF: {f.Name}");
            handler.Template = Array.Empty<byte>();
            return File(f, "application/pdf", "document.pdf");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unable to Parse Single File");
            return Problem("Unable to parse Single File");
        }
    }

    /// <summary>
    /// Get Bulk Documents
    /// </summary>
    /// <param name="handle"><see cref="HandlerDto" /></param>
    /// <returns>A link from which to download a zip file containing the bulk PDF documents.</returns>
    /// <remarks>Attempts to generate a bulk collection of PDF documents to download</remarks>
    [HttpPost]
    [Route("archive")]
    public async Task<IActionResult> Get([FromBody] HandlerDto handle)
    {
        Log.Information("Beginning Bundle");
        // Start by creating a temp directory to work within
        var directory = Directory.CreateDirectory(
            $"{Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString())}");
        try
        {
            await SetupDocument(handle);
            Log.Debug($"Created Directory: {directory.FullName}");
            // create the pdf files
            Log.Debug($"Parsing Property Collection: {handle.Properties.Count()}");
            foreach (var prop in handle.Properties)
            {
                await using var stream = await handler.GenerateAsync(prop);
                await using var file = new FileStream(Path.Join(directory.FullName, $"{Guid.NewGuid()}.pdf"),
                    FileMode.Create);
                Log.Information($"Created PDF: {file.Name}");
                await stream.CopyToAsync(file);
            }

            // zip the directory
            var archive = new FileInfo($"{Path.GetTempFileName()}.zip");
            ZipFile.CreateFromDirectory(directory.FullName, archive.FullName);
            // return the zip'd archive
            // we need a stream to return
            var zip = new DeleteAfterReadingStream(archive.FullName, FileMode.Open, FileAccess.Read);
            handler.Template = Array.Empty<byte>();
            return File(zip, "application/octet-stream", "archive.zip");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unable to Parse Bundle");
            return Problem("Unable to parse bundle");
        }
        finally
        {
            // cleanup
            Log.Debug("Cleaning up directory: {directory}", directory.FullName);
            if (directory.Exists)
                directory.Delete(true);
        }
    }

    /// <summary>
    /// Sets up the Document Handler depending on request
    /// </summary>
    /// <param name="dto"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task SetupDocument(HandlerDto dto)
    {
        Log.Debug("Setting Handler Template");
        switch (dto.Template.Length)
        {
            case 0 when string.IsNullOrWhiteSpace(dto.TemplateUrl):
                throw new ArgumentNullException(nameof(dto.TemplateUrl));
            case 0:
            {
                // we need to grab the file and parse the data
                var request = new RestRequest(dto.TemplateUrl);
                request.AddHeader("Accept", "application/pdf");
                var response = await new RestClient().ExecuteAsync(request);
                handler.Template = response.RawBytes ?? Array.Empty<byte>();
                break;
            }
            default:
                handler.Template = dto.Template;
                break;
        }
    }
}