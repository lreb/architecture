using System.Net;

namespace Facware.ModularMonolith.Api.IntegrationTests;

public class OpenApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task OpenApiDocument_IsAvailable()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task SwaggerUi_IsAvailable()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}