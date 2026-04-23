using TestMining.Platform.Host.Components;
using TestMining.Platform.Host.DeveloperEnvironment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddScoped<DeveloperEnvironmentStatusService>();
builder.Services.Configure<DeveloperEnvironmentOptions>(builder.Configuration.GetSection(DeveloperEnvironmentOptions.SectionName));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/developer-environment/status", async (DeveloperEnvironmentStatusService statusService, CancellationToken cancellationToken) =>
{
    var status = await statusService.GetStatusAsync(cancellationToken);
    return Results.Ok(status);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;
