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
    ValueTask Init(string elementId, TrustedFormConfiguration configuration, DotNetObjectReference<TrustedForm> dotNetCallback, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a MutationObserver to monitor DOM changes for the TrustedForm widget.
    /// </summary>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);
}
