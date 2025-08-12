using System.Text.Json.Serialization;

namespace Soenneker.TrustedForm.Blazor.Options;

/// <summary>
/// Configuration for TrustedForm widget/component.
/// </summary>
public sealed class TrustedFormConfiguration
{
    /// <summary>
    /// The field parameter for TrustedForm. Default is "trustedform-field-GUID".
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>
    /// Whether to invert field sensitivity. Default is false.
    /// </summary>
    [JsonPropertyName("invertFieldSensitivity")]
    public bool InvertFieldSensitivity { get; set; } = false;

    /// <summary>
    /// Whether to use sandbox mode. Default is false.
    /// </summary>
    [JsonPropertyName("sandbox")]
    public bool Sandbox { get; set; } = false;

    /// <summary>
    /// Whether to use tagged consent. Default is true.
    /// </summary>
    [JsonPropertyName("useTaggedConsent")]
    public bool UseTaggedConsent { get; set; } = true;

    /// <summary>
    /// If true, TrustedForm will not start recording automatically on load.
    /// </summary>
    [JsonPropertyName("disableRecording")]
    public bool DisableRecording { get; set; } = false;

    /// <summary>
    /// Includes an arbitrary form element within the TrustedForm component so a cert can be retrieved. This is typically used when a form isn't available. Default is false.
    /// </summary>
    [JsonIgnore]
    public bool IncludeForm { get; set; }
} 