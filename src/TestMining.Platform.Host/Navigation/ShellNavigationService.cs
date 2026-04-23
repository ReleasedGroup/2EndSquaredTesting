using System.Security.Claims;
using TestMining.Platform.Host.Auth;

namespace TestMining.Platform.Host.Navigation;

public sealed class ShellNavigationService
{
    public IReadOnlyList<ShellNavigationItem> GetVisibleItems(ClaimsPrincipal user)
    {
        return ShellNavigationCatalog.All
            .Where(item => item.IsVisibleTo(user))
            .ToArray();
    }

    public string GetPrimaryRoleLabel(ClaimsPrincipal user)
    {
        return AppRoles.All.FirstOrDefault(user.IsInRole) ?? "Unauthenticated";
    }
}
