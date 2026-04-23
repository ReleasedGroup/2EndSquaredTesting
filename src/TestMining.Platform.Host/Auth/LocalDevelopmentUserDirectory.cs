using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace TestMining.Platform.Host.Auth;

public sealed class LocalDevelopmentUserDirectory
{
    private readonly IReadOnlyDictionary<string, LocalDevelopmentUserDefinition> _users;

    public LocalDevelopmentUserDirectory(IOptions<LocalDevelopmentAuthOptions> options)
    {
        _users = options.Value.Users.ToDictionary(
            user => user.Id,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<LocalDevelopmentUserDefinition> GetUsers()
    {
        return _users.Values.OrderBy(user => user.DisplayName, StringComparer.Ordinal).ToArray();
    }

    public ClaimsPrincipal CreatePrincipal(string userId)
    {
        if (!_users.TryGetValue(userId, out var user))
        {
            throw new InvalidOperationException($"Unknown local development user '{userId}'.");
        }

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identity);
    }
}
