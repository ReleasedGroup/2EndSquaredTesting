namespace TestMining.Platform.Host.DeveloperEnvironment;

public sealed record DeveloperEnvironmentStatus(
    string EnvironmentName,
    string ArtefactStoragePath,
    DependencyCheckResult ArtefactStorage,
    DependencyCheckResult Postgres,
    DependencyCheckResult Fixtures,
    DateTimeOffset CheckedAt);

public sealed record DependencyCheckResult(
    bool Configured,
    bool Ready,
    string Summary);
