using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.StellarFederation.Features.Minting.Storage.Models;

namespace Beamable.StellarFederation.Features.Inventory.Extensions;

public static class ItemsStateExtensions
{
    public static Dictionary<string, List<FederatedItemProxy>> ToFederatedItems(this List<Mint> mints)
    {
        return mints
            .GroupBy(m => m.ContentId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => new FederatedItemProxy
                {
                    proxyId = m.TokenId.ToString(),
                    properties = new List<ItemProperty>
                        {
                            new() { name = "name", value = m.Metadata.Name },
                            new() { name = "description", value = m.Metadata.Description },
                            new() { name = "image", value = m.Metadata.Image },
                            new() { name = "symbol", value = m.Metadata.Symbol },
                            new() { name = "external_url", value = m.Metadata.ExternalUrl }
                        }
                        .Concat(
                            m.Metadata.Attributes.Select(attr => new ItemProperty
                            {
                                name = attr.Key,
                                value = attr.Value
                            })
                        )
                        .ToList()
                }).ToList()
            );
    }
}