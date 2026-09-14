namespace Facware.ModularMonolith.ArchitectureTests;

public class ApiDependencyTests
{
    [Fact]
    public void ApiProject_MustReferenceSharedKernel()
    {
        var references = ProjectReferences.GetProjectReferenceNames("src/Facware.ModularMonolith.Api/Facware.ModularMonolith.Api.csproj");

        Assert.Contains("Facware.ModularMonolith.SharedKernel.csproj", references);
    }
}