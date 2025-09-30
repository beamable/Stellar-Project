using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Extensions;

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
}