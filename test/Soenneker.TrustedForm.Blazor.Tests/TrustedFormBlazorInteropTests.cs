using Soenneker.TrustedForm.Blazor.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.TrustedForm.Blazor.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class TrustedFormBlazorInteropTests : HostedUnitTest
{
    private readonly ITrustedFormInterop _blazorlibrary;

    public TrustedFormBlazorInteropTests(Host host) : base(host)
    {
        _blazorlibrary = Resolve<ITrustedFormInterop>(true);
    }

    [Test]
    public void Default()
    {

    }
}
