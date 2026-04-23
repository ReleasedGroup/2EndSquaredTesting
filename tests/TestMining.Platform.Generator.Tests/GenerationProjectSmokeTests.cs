namespace TestMining.Platform.Generator.Tests;

public sealed class GenerationProjectSmokeTests
{
    [Fact(DisplayName = "Section 22.2 keeps the generation project wired into the scaffolded test suite")]
    public void GenerationProjectReferenceIsPresent()
    {
        Assert.Equal("TestMining.Platform.Generation", typeof(TestMining.Platform.Generation.GenerationAssemblyMarker).Assembly.GetName().Name);
    }
}
