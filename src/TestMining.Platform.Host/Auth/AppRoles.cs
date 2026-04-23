namespace TestMining.Platform.Host.Auth;

public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Author = "Author";
    public const string Viewer = "Viewer";

    public static readonly IReadOnlyList<string> All = [Administrator, Author, Viewer];
    public static readonly IReadOnlyList<string> Authoring = [Administrator, Author];
    public static readonly IReadOnlyList<string> Review = [Administrator, Author, Viewer];

    public const string AuthoringCsv = Administrator + "," + Author;
    public const string ReviewCsv = Administrator + "," + Author + "," + Viewer;

    public static bool IsKnown(string role)
    {
        return All.Contains(role, StringComparer.Ordinal);
    }
}
