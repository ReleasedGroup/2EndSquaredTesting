namespace TestMining.Platform.Fixtures;

public sealed class FixtureStateStore
{
    private readonly Lock syncRoot = new();
    private List<FixtureCustomer> customers = SeedCustomers();
    private List<string> auditTrail = SeedAuditTrail();
    private int nextCustomerId = 4;

    public FixtureStateSnapshot GetSnapshot()
    {
        lock (syncRoot)
        {
            return CreateSnapshot();
        }
    }

    public FixtureStateSnapshot Reset()
    {
        lock (syncRoot)
        {
            customers = SeedCustomers();
            auditTrail = SeedAuditTrail();
            nextCustomerId = 4;
            return CreateSnapshot();
        }
    }

    public FixtureStateSnapshot AddCustomer(CreateCustomerRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Role);

        lock (syncRoot)
        {
            customers.Add(new FixtureCustomer(nextCustomerId++, request.Name.Trim(), request.Email.Trim(), request.Role.Trim(), true));
            auditTrail.Add($"Created customer {request.Name.Trim()}.");
            return CreateSnapshot();
        }
    }

    public FixtureStateSnapshot UpdateCustomer(int id, UpdateCustomerRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Role);

        lock (syncRoot)
        {
            var index = customers.FindIndex(customer => customer.Id == id);
            if (index < 0)
            {
                throw new InvalidOperationException($"Customer {id} was not found.");
            }

            customers[index] = new FixtureCustomer(id, request.Name.Trim(), request.Email.Trim(), request.Role.Trim(), request.IsActive);
            auditTrail.Add($"Updated customer {request.Name.Trim()}.");
            return CreateSnapshot();
        }
    }

    public FixtureStateSnapshot DeleteCustomer(int id)
    {
        lock (syncRoot)
        {
            customers.RemoveAll(customer => customer.Id == id);
            auditTrail.Add($"Deleted customer {id}.");
            return CreateSnapshot();
        }
    }

    public FixtureStateSnapshot AppendAuditEvent(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        lock (syncRoot)
        {
            auditTrail.Add(message.Trim());
            return CreateSnapshot();
        }
    }

    private FixtureStateSnapshot CreateSnapshot() =>
        new(
            customers.Select(customer => customer with { }).ToArray(),
            auditTrail.ToArray());

    private static List<FixtureCustomer> SeedCustomers() =>
    [
        new FixtureCustomer(1, "Ada Lovelace", "ada@example.test", "Administrator", true),
        new FixtureCustomer(2, "Grace Hopper", "grace@example.test", "Operator", true),
        new FixtureCustomer(3, "Katherine Johnson", "katherine@example.test", "Reviewer", false)
    ];

    private static List<string> SeedAuditTrail() =>
    [
        "Seeded fixture data set.",
        "Ready for recording and replay validation."
    ];
}
