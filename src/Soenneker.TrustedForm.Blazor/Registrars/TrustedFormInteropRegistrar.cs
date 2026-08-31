using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;
using Soenneker.TrustedForm.Blazor.Abstract;

namespace Soenneker.TrustedForm.Blazor.Registrars;

/// <summary>
/// Registers TrustedForm browser interoperability for Blazor.
/// </summary>
public static class TrustedFormInteropRegistrar
{
    /// <summary>
    /// Adds <see cref="ITrustedFormInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddTrustedFormInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped().TryAddScoped<ITrustedFormInterop, TrustedFormInterop>();

        return services;
    }
}
