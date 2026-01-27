using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.ValueTask;
using Soenneker.TrustedForm.Blazor.Abstract;
using Soenneker.TrustedForm.Blazor.Options;
using Soenneker.Utils.CancellationScopes;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.TrustedForm.Blazor;

/// <inheritdoc cref="ITrustedFormInterop"/>
public sealed class TrustedFormInterop : ITrustedFormInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer _scriptInitializer;
    private readonly CancellationScope _cancellationScope = new();
    private bool _isRecording;

    private const string _modulePath = "Soenneker.TrustedForm.Blazor/js/trustedforminterop.js";
    private const string _moduleName = "TrustedFormInterop";

    public TrustedFormInterop(IJSRuntime jSRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jSRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncInitializer(async token =>
        {
            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_modulePath, _moduleName, 100, token).NoSync();
        });
    }

    public async ValueTask Init(string elementId, TrustedFormConfiguration configuration, DotNetObjectReference<TrustedForm> dotNetCallback,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.init", linked, elementId, configuration, dotNetCallback).NoSync();
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", linked, elementId).NoSync();
        }
    }

    public async ValueTask<string?> GetCertUrl(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();

            return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrl", linked, elementId);
        }
    }

    public async ValueTask<string?> GetCertUrlForSingleElement(CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();

            return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrlForSingleElement", linked);
        }
    }

    public async ValueTask Stop(CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.stop", linked).NoSync();
            _isRecording = false;
        }
    }

    public async ValueTask Start(CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", linked).NoSync();
            _isRecording = true;
        }
    }

    public async ValueTask StartIfNotRunning(CancellationToken cancellationToken = default)
    {
        if (_isRecording)
            return;

        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", linked).NoSync();
            _isRecording = true;
        }
    }

    public bool IsRecording()
    {
        return _isRecording;
    }

    public async ValueTask Finalize(string elementId, TrustedFormConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(linked).NoSync();
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.finalize", linked, elementId, configuration).NoSync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_modulePath).NoSync();
        await _scriptInitializer.DisposeAsync().NoSync();
        await _cancellationScope.DisposeAsync().NoSync();
    }
}