using System.Security.Claims;
using TestMining.Platform.Host.Auth;
using TestMining.Platform.Host.Navigation;

namespace TestMining.Platform.Host.Tests.Unit;

public sealed class ShellNavigationServiceTests
{
    private readonly ShellNavigationService _service = new();

    [Fact]
    [Trait("Requirement", "12.1")]
    [Trait("Requirement", "9.1")]
    public void GetVisibleItems_ForViewer_ShouldHideRecordingsAndAdministration()
    {
        var user = CreatePrincipal(AppRoles.Viewer);

        var items = _service.GetVisibleItems(user);
        var titles = items.Select(item => item.Title).ToArray();

        Assert.DoesNotContain("Recordings", titles);
        Assert.DoesNotContain("Administration", titles);
        Assert.Contains("Scenarios", titles);
    }

    [Fact]
    [Trait("Requirement", "12.1")]
    [Trait("Requirement", "9.1")]
    public void GetVisibleItems_ForAdministrator_ShouldShowAllAreas()
    {
        var user = CreatePrincipal(AppRoles.Administrator);

        var items = _service.GetVisibleItems(user);

        Assert.Equal(6, items.Count);
    }

    private static ClaimsPrincipal CreatePrincipal(string role)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim(ClaimTypes.Role, role)],
                "Test"));
    }
}
