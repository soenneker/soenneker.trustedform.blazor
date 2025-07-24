using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.TrustedForm.Blazor.Abstract;

/// <summary>
/// Represents the TrustedForm Blazor component with full interop functionality.
/// </summary>
public interface ITrustedForm : IAsyncDisposable
{
    /// <summary>
    /// Invoked when the TrustedForm widget is ready.
    /// </summary>
    EventCallback OnLoad { get; set; }

    /// <summary>
    /// Callback from JavaScript indicating that TrustedForm is fully loaded and ready.
    /// </summary>
    Task OnLoadCallback();

    ValueTask<string?> GetCertUrl(CancellationToken cancellationToken = default);
}