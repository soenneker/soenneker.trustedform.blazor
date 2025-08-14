using Soenneker.TrustedForm.Blazor.Abstract;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.AsyncSingleton;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.TrustedForm.Blazor.Options;

namespace Soenneker.TrustedForm.Blazor;

/// <inheritdoc cref="ITrustedFormInterop"/>
public sealed class TrustedFormInterop : ITrustedFormInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncSingleton _scriptInitializer;
    private bool _isRecording;

    private const string _modulePath = "Soenneker.TrustedForm.Blazor/js/trustedforminterop.js";
    private const string _moduleName = "TrustedFormInterop";

    public TrustedFormInterop(IJSRuntime jSRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jSRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncSingleton(async (token, _) =>
        {
            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_modulePath, _moduleName, 100, token).NoSync();
            return new object();
        });
    }

    public async ValueTask Init(string elementId, TrustedFormConfiguration configuration, DotNetObjectReference<TrustedForm> dotNetCallback,
        CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.init", cancellationToken, elementId, configuration, dotNetCallback).NoSync();
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, elementId).NoSync();
    }

    public async ValueTask<string?> GetCertUrl(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();

        return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrl", cancellationToken, elementId);
    }

    public async ValueTask<string?> GetCertUrlForSingleElement(CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();

        return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCertUrlForSingleElement", cancellationToken);
    }

    public async ValueTask Stop(CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.stop", cancellationToken).NoSync();
        _isRecording = false;
    }

    public async ValueTask Start(CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", cancellationToken).NoSync();
        _isRecording = true;
    }

    public async ValueTask StartIfNotRunning(CancellationToken cancellationToken = default)
    {
        if (_isRecording)
            return;

        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.start", cancellationToken).NoSync();
        _isRecording = true;
    }

    public bool IsRecording()
    {
        return _isRecording;
    }

    public async ValueTask Finalize(string elementId, TrustedFormConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.finalize", cancellationToken, elementId, configuration).NoSync();
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_modulePath).NoSync();
        await _scriptInitializer.DisposeAsync().NoSync();
    }
}