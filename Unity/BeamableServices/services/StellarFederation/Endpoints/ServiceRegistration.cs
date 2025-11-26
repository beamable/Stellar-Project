using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Extensions;

namespace Beamable.StellarFederation.Endpoints;

public static class ServiceRegistration
{
    public static void AddEndpoints(this IDependencyBuilder builder)
    {
        Assembly.GetExecutingAssembly()
            .GetDerivedTypes<IEndpoint>()
            .ToList()
            .ForEach(endpointType => builder.AddScoped(endpointType));
    }
}