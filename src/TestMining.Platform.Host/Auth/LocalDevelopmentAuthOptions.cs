namespace TestMining.Platform.Host.Auth;

public sealed class LocalDevelopmentAuthOptions
{
    public const string SectionName = "Authentication";
    public const string LocalDevelopmentMode = "LocalDevelopment";

    public string Mode { get; init; } = LocalDevelopmentMode;
    public IReadOnlyList<LocalDevelopmentUserDefinition> Users { get; init; } = [];
}
