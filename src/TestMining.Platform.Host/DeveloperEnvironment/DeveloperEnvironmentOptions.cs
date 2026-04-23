namespace TestMining.Platform.Host.DeveloperEnvironment;

public sealed class DeveloperEnvironmentOptions
{
    public const string SectionName = "DeveloperEnvironment";

    public string ArtefactStoragePath { get; init; } = "..\\.local\\platform-artefacts";

    public string FixtureBaseUrl { get; init; } = "http://localhost:5274";
}
