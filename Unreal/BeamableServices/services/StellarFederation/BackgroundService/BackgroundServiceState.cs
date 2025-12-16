using System;
using Beamable.Common.Dependencies;

namespace Beamable.StellarFederation.BackgroundService;

public static class BackgroundServiceState
{
    public static bool Initialized { get; set; }
    private static IDependencyProvider? MicroserviceProvider { get; set; }
    private static IServiceProvider? HostedProvider { get; set; }

    internal static void SetMicroserviceProvider(IDependencyProvider provider)
    {
        MicroserviceProvider = provider;
    }

    internal static void SetHostedProvider(IServiceProvider provider)
    {
        HostedProvider = provider;
    }

    public static T? GetFromMicroservice<T>()
    {
        var service = MicroserviceProvider?.GetService(typeof(T));
        if (service is T typedService)
            return typedService;
        return default;
    }

    public static T? GetFromHostedService<T>()
    {
        var service = HostedProvider?.GetService(typeof(T));
        if (service is T typedService)
            return typedService;
        return default;
    }

    public static void ResetDelay()
    {
        GetFromHostedService<StellarBackgroundService>()?.ResetDelay();
    }
}