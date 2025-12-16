using System;
using Beamable.Common.Content;
using Beamable.Common.Dependencies;
using Beamable.Common.Inventory;
using Microsoft.Extensions.DependencyInjection;
using StellarFederationCommon.FederationContent;

namespace Beamable.StellarFederation.Features.Inventory.Handlers;

public class ContentHandlerFactory(
    IDependencyProvider serviceProvider) : IService
{
    public IContentHandler? GetHandler(IContentObject contentObject)
    {
        return contentObject switch
        {
            CoinCurrency coinCurrency => serviceProvider.GetRequiredService<RegularCoinHandler>(),
            GoldCurrency goldCurrency => serviceProvider.GetRequiredService<SupplyCoinHandler>(),
            ItemContent item => serviceProvider.GetRequiredService<ItemStateHandler>(),
            //_ => throw new NotSupportedException($"ContentId '{contentObject.Id}' is not supported.")
            _ => null
        };
    }
}