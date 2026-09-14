using System.Xml.Linq;

namespace Facware.ModularMonolith.ArchitectureTests;

internal static class ProjectReferences
{
    public static IReadOnlyList<string> GetProjectReferenceNames(string projectPath)
    {
        var fullPath = Path.Combine(GetRepositoryRoot(), projectPath);
        var document = XDocument.Load(fullPath);

        return document
            .Descendants()
            .Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => value is not null)
            .Select(value => Path.GetFileName(value!.Replace('\\', Path.DirectorySeparatorChar)))
            .ToList();
    }

    private static string GetRepositoryRoot()
    {
        var current = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(current);

        while (directory is not null && !Directory.GetFiles(directory.FullName, "*.slnx").Any())
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new DirectoryNotFoundException("Could not locate the repository root (.slnx file).");
        }

        return directory.FullName;
    }
}