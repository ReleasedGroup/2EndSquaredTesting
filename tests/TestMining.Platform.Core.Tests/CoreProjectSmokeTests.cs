namespace TestMining.Platform.Core.Tests;

public sealed class CoreProjectSmokeTests
{
    [Fact(DisplayName = "Section 22.2 keeps the core project wired into the scaffolded test suite")]
    public void CoreProjectReferenceIsPresent()
    {
        Assert.Equal("TestMining.Platform.Core", typeof(TestMining.Platform.Core.CoreAssemblyMarker).Assembly.GetName().Name);
    }
}
