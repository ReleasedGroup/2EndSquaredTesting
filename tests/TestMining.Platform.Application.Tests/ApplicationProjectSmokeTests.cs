namespace TestMining.Platform.Application.Tests;

public sealed class ApplicationProjectSmokeTests
{
    [Fact(DisplayName = "Section 22.2 keeps the application project wired into the scaffolded test suite")]
    public void ApplicationProjectReferenceIsPresent()
    {
        Assert.Equal("TestMining.Platform.Application", typeof(TestMining.Platform.Application.ApplicationAssemblyMarker).Assembly.GetName().Name);
    }
}
