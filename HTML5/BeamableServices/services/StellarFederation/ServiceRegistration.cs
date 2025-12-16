using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.BackgroundService;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Beamable.StellarFederation;

public static class ServiceRegistration
{
    public static void AddFeatures(this IDependencyBuilder builder)
    {
        Assembly.GetExecutingAssembly()
            .GetDerivedTypes<IService>()
            .ToList()
            .ForEach(serviceType => builder.AddSingleton(serviceType));
    }

    public static void AddBackgroundServiceFeatures(this IServiceCollection services)
    {
        services.AddSingleton<BackoffDelayManager>();
    }
}