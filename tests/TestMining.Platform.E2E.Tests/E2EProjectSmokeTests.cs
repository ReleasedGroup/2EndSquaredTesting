namespace TestMining.Platform.E2E.Tests;

public sealed class E2EProjectSmokeTests
{
    [Fact(DisplayName = "Section 22.2 keeps the host and fixtures projects wired into the scaffolded E2E suite")]
    public void HostAndFixturesProjectReferencesArePresent()
    {
        Assert.Equal("TestMining.Platform.Host", typeof(TestMining.Platform.Host.HostAssemblyMarker).Assembly.GetName().Name);
        Assert.Equal("TestMining.Platform.Fixtures", typeof(TestMining.Platform.Fixtures.FixturesAssemblyMarker).Assembly.GetName().Name);
    }
}
