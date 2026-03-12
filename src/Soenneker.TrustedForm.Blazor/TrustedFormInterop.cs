using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
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
            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_modulePath, _moduleName, 100, token);
        });
    }

    public async ValueTask Init(string elementId, TrustedFormConfiguration configuration, DotNetObjectReference<TrustedForm> dotNetCallback,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.init", linked, elementId, configuration, dotNetCallback);
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", linked, elementId);
        }
    }

    public async ValueTask<string?> GetCertUrl(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);

            return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrl", linked, elementId);
        }
    }

    public async ValueTask<string?> GetCertUrlForSingleElement(CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);

            return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrlForSingleElement", linked);
        }
    }

    public async ValueTask Stop(CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.stop", linked);
            _isRecording = false;
        }
    }

    public async ValueTask Start(CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", linked);
            _isRecording = true;
        }
    }

    public async ValueTask StartIfNotRunning(CancellationToken cancellationToken = default)
    {
        if (_isRecording)
            return;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", linked);
            _isRecording = true;
        }
    }

    public bool IsRecording()
    {
        return _isRecording;
    }

    public async ValueTask Finalize(string elementId, TrustedFormConfiguration configuration, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(linked);
            await _jsRuntime.InvokeVoidAsync($"{_moduleName}.finalize", linked, elementId, configuration);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_modulePath);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}