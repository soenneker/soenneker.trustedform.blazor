using Soenneker.TrustedForm.Blazor.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.TrustedForm.Blazor.Tests;

[Collection("Collection")]
public sealed class TrustedFormBlazorInteropTests : FixturedUnitTest
{
    private readonly ITrustedFormInterop _blazorlibrary;

    public TrustedFormBlazorInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _blazorlibrary = Resolve<ITrustedFormInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
