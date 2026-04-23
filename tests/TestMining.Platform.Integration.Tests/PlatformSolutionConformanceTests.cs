using System.Xml.Linq;

namespace TestMining.Platform.Integration.Tests;

public sealed class PlatformSolutionConformanceTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact(DisplayName = "Sections 17 and 22.1 require the TestMining.Platform vertical slice to exist as a distinct scaffold")]
    public void PlatformProjectsExistInTheRepositorySolution()
    {
        var solutionDocument = XDocument.Load(Path.Combine(RepositoryRoot, "Symphony.slnx"));
        var projectPaths = solutionDocument
            .Descendants("Project")
            .Select(node => node.Attribute("Path")?.Value)
            .Where(path => path is not null && path.Contains("TestMining.Platform", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var expectedPaths = GetExpectedProjectPaths()
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedPaths, projectPaths);
    }

    [Fact(DisplayName = "Sections 17, 22.2, and 23.1 item 6 keep platform project references aligned with the intended boundaries")]
    public void PlatformProjectsUseTheExpectedReferenceGraph()
    {
        var expectedReferenceGraph = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["src/TestMining.Platform.Application/TestMining.Platform.Application.csproj"] =
            [
                "src/TestMining.Platform.Contracts/TestMining.Platform.Contracts.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Analysis/TestMining.Platform.Analysis.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Artefacts/TestMining.Platform.Artefacts.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Contracts/TestMining.Platform.Contracts.csproj"] = [],
            ["src/TestMining.Platform.Core/TestMining.Platform.Core.csproj"] = [],
            ["src/TestMining.Platform.Generation/TestMining.Platform.Generation.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Healing/TestMining.Platform.Healing.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Host/TestMining.Platform.Host.csproj"] =
            [
                "src/TestMining.Platform.Analysis/TestMining.Platform.Analysis.csproj",
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Artefacts/TestMining.Platform.Artefacts.csproj",
                "src/TestMining.Platform.Contracts/TestMining.Platform.Contracts.csproj",
                "src/TestMining.Platform.Generation/TestMining.Platform.Generation.csproj",
                "src/TestMining.Platform.Healing/TestMining.Platform.Healing.csproj",
                "src/TestMining.Platform.Persistence/TestMining.Platform.Persistence.csproj",
                "src/TestMining.Platform.Recording/TestMining.Platform.Recording.csproj",
                "src/TestMining.Platform.Replay/TestMining.Platform.Replay.csproj",
            ],
            ["src/TestMining.Platform.Persistence/TestMining.Platform.Persistence.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
            ["src/TestMining.Platform.Recorder/TestMining.Platform.Recorder.csproj"] = [],
            ["src/TestMining.Platform.Recording/TestMining.Platform.Recording.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
                "src/TestMining.Platform.Recorder/TestMining.Platform.Recorder.csproj",
            ],
            ["src/TestMining.Platform.Replay/TestMining.Platform.Replay.csproj"] =
            [
                "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
                "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
            ],
        };

        foreach (var (projectPath, expectedReferences) in expectedReferenceGraph)
        {
            var projectDocument = XDocument.Load(Path.Combine(RepositoryRoot, projectPath));
            var projectDirectory = Path.GetDirectoryName(projectPath)
                ?? throw new InvalidOperationException($"Unable to resolve a directory for '{projectPath}'.");

            var actualReferences = projectDocument
                .Descendants("ProjectReference")
                .Select(node => node.Attribute("Include")?.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => NormalizePath(Path.GetRelativePath(
                    RepositoryRoot,
                    Path.GetFullPath(Path.Combine(RepositoryRoot, projectDirectory, value!)))))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            var expectedNormalizedReferences = expectedReferences
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expectedNormalizedReferences, actualReferences);
        }
    }

    [Fact(DisplayName = "Sections 22.1 and 22.2 require shared nullable, deterministic, and analyzer conventions across the platform scaffold")]
    public void PlatformProjectsInheritSharedRepositoryBuildConventions()
    {
        var directoryBuildProps = XDocument.Load(Path.Combine(RepositoryRoot, "Directory.Build.props"));

        Assert.Equal("enable", GetProjectPropertyValue(directoryBuildProps, "Nullable"));
        Assert.Equal("true", GetProjectPropertyValue(directoryBuildProps, "Deterministic"));
        Assert.Equal("true", GetProjectPropertyValue(directoryBuildProps, "EnableNETAnalyzers"));
        Assert.Equal("latest-recommended", GetProjectPropertyValue(directoryBuildProps, "AnalysisLevel"));

        foreach (var projectPath in GetExpectedProjectPaths())
        {
            var projectDocument = XDocument.Load(Path.Combine(RepositoryRoot, projectPath));

            Assert.NotEqual("disable", GetProjectPropertyValue(projectDocument, "Nullable"));
            Assert.NotEqual("false", GetProjectPropertyValue(projectDocument, "Deterministic"));
            Assert.NotEqual("false", GetProjectPropertyValue(projectDocument, "EnableNETAnalyzers"));
        }
    }

    private static string[] GetExpectedProjectPaths() =>
    [
        "src/TestMining.Platform.Analysis/TestMining.Platform.Analysis.csproj",
        "src/TestMining.Platform.Application/TestMining.Platform.Application.csproj",
        "src/TestMining.Platform.Artefacts/TestMining.Platform.Artefacts.csproj",
        "src/TestMining.Platform.Contracts/TestMining.Platform.Contracts.csproj",
        "src/TestMining.Platform.Core/TestMining.Platform.Core.csproj",
        "src/TestMining.Platform.Generation/TestMining.Platform.Generation.csproj",
        "src/TestMining.Platform.Healing/TestMining.Platform.Healing.csproj",
        "src/TestMining.Platform.Host/TestMining.Platform.Host.csproj",
        "src/TestMining.Platform.Persistence/TestMining.Platform.Persistence.csproj",
        "src/TestMining.Platform.Recorder/TestMining.Platform.Recorder.csproj",
        "src/TestMining.Platform.Recording/TestMining.Platform.Recording.csproj",
        "src/TestMining.Platform.Replay/TestMining.Platform.Replay.csproj",
        "tests/TestMining.Platform.Application.Tests/TestMining.Platform.Application.Tests.csproj",
        "tests/TestMining.Platform.Core.Tests/TestMining.Platform.Core.Tests.csproj",
        "tests/TestMining.Platform.E2E.Tests/TestMining.Platform.E2E.Tests.csproj",
        "tests/TestMining.Platform.Fixtures/TestMining.Platform.Fixtures.csproj",
        "tests/TestMining.Platform.Generator.Tests/TestMining.Platform.Generator.Tests.csproj",
        "tests/TestMining.Platform.Integration.Tests/TestMining.Platform.Integration.Tests.csproj",
    ];

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null && !File.Exists(Path.Combine(current.FullName, "AGENTS.md")))
        {
            current = current.Parent;
        }

        return current?.FullName ?? throw new InvalidOperationException("Unable to locate the repository root.");
    }

    private static string? GetProjectPropertyValue(XDocument document, string propertyName) =>
        document
            .Root?
            .Elements("PropertyGroup")
            .Elements(propertyName)
            .Select(node => node.Value.Trim())
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

    private static string NormalizePath(string path) => path.Replace('\\', '/');
}
