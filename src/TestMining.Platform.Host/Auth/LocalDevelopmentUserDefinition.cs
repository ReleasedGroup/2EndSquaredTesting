namespace TestMining.Platform.Host.Auth;

public sealed class LocalDevelopmentUserDefinition
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
