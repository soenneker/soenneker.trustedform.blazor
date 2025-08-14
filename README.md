[![](https://img.shields.io/nuget/v/soenneker.trustedform.blazor.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.trustedform.blazor/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.trustedform.blazor/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.trustedform.blazor/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.trustedform.blazor.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.trustedform.blazor/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.TrustedForm.Blazor
### A Blazor interop library for ActiveProspect TrustedForm

## Installation

```
dotnet add package Soenneker.TrustedForm.Blazor
```

## Configuration

The TrustedForm component supports various configuration options to customize its behavior:

```csharp
var configuration = new TrustedFormConfiguration
{
    Debug = true,
    IncludeForm = true,
    Sandbox = true
};
```