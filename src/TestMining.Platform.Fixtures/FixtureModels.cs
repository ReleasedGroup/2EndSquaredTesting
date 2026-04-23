namespace TestMining.Platform.Fixtures;

public sealed record FixtureCustomer(int Id, string Name, string Email, string Role, bool IsActive);

public sealed record FixtureStateSnapshot(
    IReadOnlyList<FixtureCustomer> Customers,
    IReadOnlyList<string> AuditTrail);

public sealed record CreateCustomerRequest(string Name, string Email, string Role);

public sealed record UpdateCustomerRequest(string Name, string Email, string Role, bool IsActive);

public sealed record AuditEventRequest(string Message);

public sealed record LoginRequest(string UserName, string Password);

public sealed record SessionSnapshot(bool Authenticated, string? UserName);
