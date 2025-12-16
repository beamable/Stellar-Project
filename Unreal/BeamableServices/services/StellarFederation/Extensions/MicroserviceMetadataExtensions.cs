using System;
using System.Collections.Concurrent;
using System.Reflection;
using Beamable.Common;
using Beamable.Common.Api.Auth;
using Beamable.Server;
using StellarFederationCommon;

namespace Beamable.StellarFederation.Extensions;

public static class MicroserviceMetadataExtensions
{
    private static readonly ConcurrentDictionary<Type, MicroserviceInfo> Cache = new();

    public static MicroserviceInfo GetMetadata<TService, TIdentity>()
        where TService : Microservice, IFederatedLogin<TIdentity>, new()
        where TIdentity : IFederationId, new()
    {
        return Cache.GetOrAdd(typeof(TIdentity), _ =>
        {
            var microservice = new TService();
            var microserviceName = microservice.GetType().GetCustomAttribute<MicroserviceAttribute>()!.MicroserviceName;
            var identity = new TIdentity();
            var microserviceNamespace = identity.GetUniqueName();
            return new MicroserviceInfo(microserviceName, microserviceNamespace);
        });
    }

    public static ExternalIdentity ToExternalIdentity(this MicroserviceInfo microserviceInfo)
        => new()
        {
            providerService = microserviceInfo.MicroserviceName,
            providerNamespace = microserviceInfo.MicroserviceNamespace
        };

    public static bool IsLocalDevInstance()
    {
        return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NAME_PREFIX"));
    }
}

public record MicroserviceInfo(string MicroserviceName, string MicroserviceNamespace)
{
    public static MicroserviceInfo Default()
        => new (StellarFederationSettings.MicroserviceName,
            StellarFederationSettings.StellarIdentityName);
}