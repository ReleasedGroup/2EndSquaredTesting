using Microsoft.Extensions.Options;
using TestMining.Platform.Host.Auth;

namespace TestMining.Platform.Host.Tests.Unit;

public sealed class LocalDevelopmentAuthOptionsValidatorTests
{
    private readonly LocalDevelopmentAuthOptionsValidator _validator = new();

    [Fact]
    [Trait("Requirement", "11.3")]
    [Trait("Requirement", "12.1")]
    public void Validate_ShouldFailForUnknownRole()
    {
        var options = new LocalDevelopmentAuthOptions
        {
            Users =
            [
                new LocalDevelopmentUserDefinition
                {
                    Id = "bad-role",
                    DisplayName = "Bad Role",
                    Email = "bad@local.test",
                    Role = "Operator",
                    Description = "Invalid"
                }
            ]
        };

        var result = _validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains("Unknown local development role", string.Join(' ', result.Failures), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Requirement", "11.3")]
    [Trait("Requirement", "12.1")]
    public void Validate_ShouldFailForExternalMode()
    {
        var options = new LocalDevelopmentAuthOptions
        {
            Mode = "External",
            Users =
            [
                new LocalDevelopmentUserDefinition
                {
                    Id = "administrator",
                    DisplayName = "Alex Admin",
                    Email = "alex@local.test",
                    Role = AppRoles.Administrator,
                    Description = "Administrator"
                }
            ]
        };

        var result = _validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains("LocalDevelopment", string.Join(' ', result.Failures), StringComparison.OrdinalIgnoreCase);
    }
}
