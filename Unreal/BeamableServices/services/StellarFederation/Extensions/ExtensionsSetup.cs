using Beamable.Server;

namespace Beamable.StellarFederation.Extensions;

public static class ExtensionsSetup
{
    public static void SetupExtensions(this IServiceInitializer initializer)
    {
        initializer.SetupMongoExtensions();
    }
}