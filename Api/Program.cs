using Api.Services.Contracts;
using Api.Services.Implementations;
using Azure.Identity;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) =>
    config.ReadFrom.Configuration(builder.Configuration));
builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["DataDogVault"]!), new DefaultAzureCredential());

if (!builder.Environment.IsProduction())
    IdentityModelEventSource.ShowPII = true;
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddTransient<IHandler, DocumentHandler>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Startup "Configure"
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (ctx, _, ex) => ex != null
        ? LogEventLevel.Error
        : ctx.Response.StatusCode > 499
            ? LogEventLevel.Error
            : LogEventLevel.Debug;
});
app.MapHealthChecks("/heartbeat");
app.MapControllers();
app.Run();