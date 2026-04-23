using Microsoft.Extensions.Options;

namespace TestMining.Platform.Host.Auth;

public sealed class LocalDevelopmentAuthOptionsValidator
    : IValidateOptions<LocalDevelopmentAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, LocalDevelopmentAuthOptions options)
    {
        if (!string.Equals(
                options.Mode,
                LocalDevelopmentAuthOptions.LocalDevelopmentMode,
                StringComparison.Ordinal))
        {
            return ValidateOptionsResult.Fail(
                "Only LocalDevelopment authentication mode is supported in this slice. " +
                "External host authentication remains deferred pending the authentication ADR.");
        }

        if (options.Users.Count == 0)
        {
            return ValidateOptionsResult.Fail(
                "At least one local development user must be configured.");
        }

        var duplicateIds = options.Users
            .GroupBy(user => user.Id, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicateIds.Length > 0)
        {
            return ValidateOptionsResult.Fail(
                $"Duplicate local development user ids are not allowed: {string.Join(", ", duplicateIds)}.");
        }

        var unknownRoles = options.Users
            .Where(user => !AppRoles.IsKnown(user.Role))
            .Select(user => user.Role)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (unknownRoles.Length > 0)
        {
            return ValidateOptionsResult.Fail(
                $"Unknown local development role(s): {string.Join(", ", unknownRoles)}.");
        }

        if (options.Users.Any(user =>
                string.IsNullOrWhiteSpace(user.Id) ||
                string.IsNullOrWhiteSpace(user.DisplayName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Description)))
        {
            return ValidateOptionsResult.Fail(
                "Each local development user must include id, display name, email, and description.");
        }

        return ValidateOptionsResult.Success;
    }
}
