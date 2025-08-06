using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.TrustedForm.Blazor.Options;

namespace Soenneker.TrustedForm.Blazor.Abstract;

/// <summary>
/// An interop utility for loading the ActiveProspect TrustedForm script.
/// </summary>
public interface ITrustedFormInterop : IAsyncDisposable
{
    /// <summary>
    /// Loads the TrustedForm script with the given configuration and sets up callbacks.
    /// </summary>
    ValueTask Init(string elementId, TrustedFormConfiguration configuration, DotNetObjectReference<TrustedForm> dotNetCallback,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a MutationObserver to monitor DOM changes for the TrustedForm widget.
    /// </summary>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the TrustedForm cert URL for the given element.
    /// </summary>
    ValueTask<string?> GetCertUrl(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops TrustedForm recording.
    /// </summary>
    ValueTask Stop(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts TrustedForm recording.
    /// </summary>
    ValueTask Start(CancellationToken cancellationToken = default);
}