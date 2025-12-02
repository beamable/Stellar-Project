using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Extensions;

namespace Beamable.StellarFederation.Endpoints;

public static class ServiceRegistration
{
    public static void AddEndpoints(this IDependencyBuilder builder)
    {
        builder.AddScoped<AuthenticateEndpoint>();
        builder.AddScoped<StartInventoryTransactionEndpoint>();
        builder.AddScoped<GetInventoryStateEndpoint>();
        builder.AddScoped<AuthenticateExternalEndpoint>();
    }
}