using System;
using Beamable.Common.Dependencies;
using Beamable.Common.Inventory;
using Beamable.StellarFederation.Features.Contract.Models;
using StellarFederationCommon.FederationContent;

namespace Beamable.StellarFederation.Features.Contract.Handlers;

public class ContentContractHandlerResolver(IDependencyProvider dependencyProvider) : IService
{
    public IContentContractHandler Resolve(ContentContractsModel model)
    {
        ContractTemplateService.Initialize();
        return model.ContentObject switch
        {
            CoinCurrency => dependencyProvider.GetService<CoinCurrencyHandler>(),
            GoldCurrency => dependencyProvider.GetService<GoldCurrencyHandler>(),
            ItemContent => dependencyProvider.GetService<ItemHandler>(),
            _ => throw new InvalidOperationException($"No handler found for content type: {model.ContentObject.GetType().Name}")
        };
    }
}