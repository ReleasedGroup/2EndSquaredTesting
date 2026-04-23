namespace TestMining.Platform.Host.Auth;

public static class RedirectTarget
{
    public static string ForReturnUrl(string? returnUrl, string defaultPath = "/dashboard")
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return defaultPath;
        }

        if (!returnUrl.StartsWith("/", StringComparison.Ordinal) ||
            returnUrl.StartsWith("//", StringComparison.Ordinal))
        {
            return defaultPath;
        }

        return returnUrl;
    }
}
