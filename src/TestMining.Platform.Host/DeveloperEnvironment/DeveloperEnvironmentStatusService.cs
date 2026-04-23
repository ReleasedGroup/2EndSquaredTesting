using Microsoft.Extensions.Options;
using Npgsql;

namespace TestMining.Platform.Host.DeveloperEnvironment;

public sealed class DeveloperEnvironmentStatusService(
    IConfiguration configuration,
    IHostEnvironment hostEnvironment,
    IHttpClientFactory httpClientFactory,
    IOptions<DeveloperEnvironmentOptions> options)
{
    private static readonly TimeSpan DependencyTimeout = TimeSpan.FromMilliseconds(250);

    public async Task<DeveloperEnvironmentStatus> GetStatusAsync(CancellationToken cancellationToken)
    {
        var artefactStoragePath = ResolveArtefactStoragePath(options.Value.ArtefactStoragePath);
        var artefactStorage = CheckArtefactStorage(artefactStoragePath);
        var postgres = await CheckPostgresAsync(configuration.GetConnectionString("PlatformPostgres"), cancellationToken);
        var fixtures = await CheckFixturesAsync(options.Value.FixtureBaseUrl, cancellationToken);

        return new DeveloperEnvironmentStatus(
            hostEnvironment.EnvironmentName,
            artefactStoragePath,
            artefactStorage,
            postgres,
            fixtures,
            DateTimeOffset.UtcNow);
    }

    private string ResolveArtefactStoragePath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        return Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, configuredPath));
    }

    private static DependencyCheckResult CheckArtefactStorage(string artefactStoragePath)
    {
        Directory.CreateDirectory(artefactStoragePath);

        return new DependencyCheckResult(
            Configured: true,
            Ready: true,
            Summary: "Ready for local artefact output.");
    }

    private static async Task<DependencyCheckResult> CheckPostgresAsync(string? connectionString, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new DependencyCheckResult(
                Configured: false,
                Ready: false,
                Summary: "Set ConnectionStrings__PlatformPostgres before running the local profile.");
        }

        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Timeout = (int)Math.Ceiling(DependencyTimeout.TotalSeconds),
                CommandTimeout = (int)Math.Ceiling(DependencyTimeout.TotalSeconds)
            };

            await using var connection = new NpgsqlConnection(builder.ConnectionString);
            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(DependencyTimeout);

            await connection.OpenAsync(timeoutSource.Token);
            await using var command = new NpgsqlCommand("select 1", connection);
            await command.ExecuteScalarAsync(timeoutSource.Token);

            return new DependencyCheckResult(
                Configured: true,
                Ready: true,
                Summary: "Connection probe succeeded.");
        }
        catch (Exception ex) when (ex is NpgsqlException or TimeoutException or OperationCanceledException)
        {
            return new DependencyCheckResult(
                Configured: true,
                Ready: false,
                Summary: $"Connection probe failed: {ex.GetType().Name}.");
        }
    }

    private async Task<DependencyCheckResult> CheckFixturesAsync(string? fixtureBaseUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fixtureBaseUrl))
        {
            return new DependencyCheckResult(
                Configured: false,
                Ready: false,
                Summary: "Set DeveloperEnvironment:FixtureBaseUrl before running fixture validation.");
        }

        try
        {
            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(DependencyTimeout);

            var client = httpClientFactory.CreateClient();
            client.Timeout = DependencyTimeout;

            using var response = await client.GetAsync($"{fixtureBaseUrl.TrimEnd('/')}/healthz", timeoutSource.Token);

            return response.IsSuccessStatusCode
                ? new DependencyCheckResult(true, true, "Fixture app responded to the local health check.")
                : new DependencyCheckResult(true, false, $"Fixture app returned HTTP {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TimeoutException or TaskCanceledException)
        {
            return new DependencyCheckResult(
                Configured: true,
                Ready: false,
                Summary: $"Fixture health probe failed: {ex.GetType().Name}.");
        }
    }
}
