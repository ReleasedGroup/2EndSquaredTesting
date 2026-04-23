using System.Security.Claims;

namespace TestMining.Platform.Host.Navigation;

public sealed record ShellNavigationItem(
    string Route,
    string Title,
    string NavHint,
    string Summary,
    string ActionHint,
    string ReadOnlyHint,
    string ReadOnlyExplanation,
    string PrimaryActionLabel,
    string ActionNote,
    IReadOnlyList<string> VisibleRoles,
    IReadOnlyList<string> ActionRoles,
    IReadOnlyList<string> VisibleNow)
{
    public string Overview => Summary;

    public bool IsVisibleTo(ClaimsPrincipal user)
    {
        return VisibleRoles.Any(user.IsInRole);
    }

    public bool AllowsActionsFor(ClaimsPrincipal user)
    {
        return ActionRoles.Any(user.IsInRole);
    }
}
