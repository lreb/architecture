using Facware.ModularMonolith.Api.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = builder.Configuration["Swagger:Title"] ?? "Facware Modular Monolith API";
        document.Info.Version = builder.Configuration["Swagger:Version"] ?? "v1";
        return Task.CompletedTask;
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddBaseHealthChecks();
builder.Services.AddGlobalExceptionHandling();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Facware Modular Monolith API v1"));
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
});

app.Run();

public partial class Program;