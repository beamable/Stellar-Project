using System;
using Beamable.Common.Content;
using Beamable.Common.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using StellarFederationCommon.FederationContent;

namespace Beamable.StellarFederation.Features.Content.Handlers;

public class ContentHandlerFactory(
    IDependencyProvider serviceProvider) : IService
{
    public IContentHandler GetHandler(IContentObject contentObject)
    {
        return contentObject switch
        {
            CoinCurrency coinCurrency => serviceProvider.GetRequiredService<RegularCoinHandler>(),
            _ => throw new NotSupportedException($"ContentId '{contentObject.Id}' is not supported.")
        };
    }
}