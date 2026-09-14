using Facware.ModularMonolith.SharedKernel.Domain;
using NetArchTest.Rules;

namespace Facware.ModularMonolith.ArchitectureTests;

public class SharedKernelDependencyTests
{
    private static readonly string[] InfrastructureNamespaces =
    [
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "Dapper",
        "Npgsql",
        "StackExchange.Redis",
    ];

    [Fact]
    public void DomainTypes_MustNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(Entity<>).Assembly)
            .That()
            .ResideInNamespace("Facware.ModularMonolith.SharedKernel.Domain")
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespaces)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void SharedKernel_MustNotReferenceApiProject()
    {
        var references = ProjectReferences.GetProjectReferenceNames(
            "src/Facware.ModularMonolith.SharedKernel/Facware.ModularMonolith.SharedKernel.csproj");

        Assert.DoesNotContain("Facware.ModularMonolith.Api.csproj", references);
    }
}