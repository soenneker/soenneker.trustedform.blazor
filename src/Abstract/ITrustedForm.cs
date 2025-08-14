using Microsoft.AspNetCore.Components;
using Soenneker.Quark.Components.Cancellable.Abstract;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.TrustedForm.Blazor.Abstract;

/// <summary>
/// Represents the TrustedForm Blazor component with full interop functionality.
/// </summary>
public interface ITrustedForm : ICancellableComponent
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

    /// <summary>
    /// Starts TrustedForm recording if it's not already running.
    /// </summary>
    ValueTask StartIfNotRunning(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current state of TrustedForm recording.
    /// </summary>
    /// <returns>True if recording is currently active, false otherwise.</returns>
    bool IsRecording();

    /// <summary>
    /// Finalizes the TrustedForm certificate for 90-day retention.
    /// This extends the retention period from the default 30 days to 90 days.
    /// </summary>
    ValueTask Finalize(CancellationToken cancellationToken = default);
}