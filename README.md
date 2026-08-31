[![](https://img.shields.io/nuget/v/soenneker.trustedform.blazor.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.trustedform.blazor/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.trustedform.blazor/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.trustedform.blazor/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.trustedform.blazor.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.trustedform.blazor/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.trustedform.blazor/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.trustedform.blazor/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.TrustedForm.Blazor
Loads ActiveProspect TrustedForm in Blazor and exposes recording, certificate URL, and finalization controls.

## Installation

```bash
dotnet add package Soenneker.TrustedForm.Blazor
```

## Registration

```csharp
using Soenneker.TrustedForm.Blazor.Registrars;

builder.Services.AddTrustedFormInteropAsScoped();
```

## Component usage

```razor
@using Soenneker.TrustedForm.Blazor
@using Soenneker.TrustedForm.Blazor.Options

<TrustedForm @ref="_trustedForm"
             Configuration="_configuration"
             OnLoad="OnTrustedFormLoaded">
    <label>
        Email
        <input name="email" type="email" />
    </label>
</TrustedForm>

@code {
    private TrustedForm? _trustedForm;

    private readonly TrustedFormConfiguration _configuration = new()
    {
        Sandbox = true,
        UseTaggedConsent = true
    };

    private async Task Submit(CancellationToken cancellationToken)
    {
        string? certificateUrl =
            await _trustedForm!.GetCertUrl(cancellationToken);

        // Submit the certificate URL with the lead data.
    }

    private Task OnTrustedFormLoaded() => Task.CompletedTask;
}
```

Use the exact certificate URL returned by `GetCertUrl()` when submitting the lead. `OnLoad` means the TrustedForm script initialized; it does not mean a certificate URL is already available.

## Configuration

`Sandbox` should be enabled outside production. `DisableRecording` prevents automatic recording; call `Start()` or `StartIfNotRunning()` when consent has been obtained. `Field` overrides the generated hidden certificate-field name, and `UseTaggedConsent` enables TrustedForm's tagged-consent behavior.

```csharp
var configuration = new TrustedFormConfiguration
{
    Debug = true,
    Sandbox = true,
    DisableRecording = true,
    UseTaggedConsent = true
};
```

Set `IncludeForm = true` only when the component should create its own form around `ChildContent`. `Finalize()` submits that internal form through a hidden button, so it requires `IncludeForm`; do not place another `EditForm` or HTML `<form>` inside it. When your application already owns the form, leave `IncludeForm` disabled and retrieve the certificate URL before normal submission.

`ProblemDetails`, `ProblemDetailsChanged`, and `ProblemDetailsContent` expose script-initialization failures. Avoid enabling `Debug` in production because browser-console output can reveal integration details.
