using TestMining.Platform.Fixtures;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FixtureStateStore>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/test/state", (FixtureStateStore store) => Results.Ok(store.GetSnapshot()));
app.MapPost("/api/test/reset", (FixtureStateStore store) => Results.Ok(store.Reset()));
app.MapGet("/api/customers", (FixtureStateStore store) => Results.Ok(store.GetSnapshot().Customers));
app.MapPost("/api/customers", (CreateCustomerRequest request, FixtureStateStore store) => Results.Ok(store.AddCustomer(request)));
app.MapPut("/api/customers/{id:int}", (int id, UpdateCustomerRequest request, FixtureStateStore store) => Results.Ok(store.UpdateCustomer(id, request)));
app.MapDelete("/api/customers/{id:int}", (int id, FixtureStateStore store) => Results.Ok(store.DeleteCustomer(id)));
app.MapPost("/api/audit-events", (AuditEventRequest request, FixtureStateStore store) => Results.Ok(store.AppendAuditEvent(request.Message)));

app.MapGet("/api/session", (HttpContext context) =>
{
    var currentUser = context.Request.Cookies.TryGetValue("fixture-auth", out var userName) ? userName : null;
    return Results.Ok(new SessionSnapshot(currentUser is not null, currentUser));
});

app.MapPost("/api/login", (LoginRequest request, HttpContext context) =>
{
    if (!string.Equals(request.UserName, "demo", StringComparison.OrdinalIgnoreCase) || request.Password != "demo")
    {
        return Results.BadRequest(new { message = "Use demo / demo for the fixture login." });
    }

    context.Response.Cookies.Append(
        "fixture-auth",
        request.UserName,
        new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

    return Results.Ok(new SessionSnapshot(true, request.UserName));
});

app.MapPost("/api/logout", (HttpContext context) =>
{
    context.Response.Cookies.Delete("fixture-auth");
    return Results.Ok(new SessionSnapshot(false, null));
});

app.MapGet("/forms", () => Results.Redirect("/forms.html"));
app.MapGet("/grid", () => Results.Redirect("/grid.html"));
app.MapGet("/modal", () => Results.Redirect("/modal.html"));
app.MapGet("/login", () => Results.Redirect("/login.html"));

app.Run();

public partial class Program;
