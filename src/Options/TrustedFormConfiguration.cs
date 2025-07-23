namespace Soenneker.TrustedForm.Blazor.Options;

/// <summary>
/// Configuration for TrustedForm widget/component.
/// </summary>
public sealed class TrustedFormConfiguration
{
    /// <summary>
    /// The field parameter for TrustedForm. Default is 'xxTrustedFormCertUrl'.
    /// </summary>
    public string Field { get; set; } = "xxTrustedFormCertUrl";

    /// <summary>
    /// Whether to invert field sensitivity. Default is false.
    /// </summary>
    public bool InvertFieldSensitivity { get; set; } = false;

    /// <summary>
    /// Whether to use sandbox mode. Default is false.
    /// </summary>
    public bool Sandbox { get; set; } = false;

    /// <summary>
    /// Whether to use tagged consent. Default is false.
    /// </summary>
    public bool UseTaggedConsent { get; set; } = false;
} 