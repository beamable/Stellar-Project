using System;
using System.Collections.Generic;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Endpoints.Commerce;
using StellarFederationCommon.FederationContent;
using StellarFederationCommon.Store;

namespace Beamable.StellarFederation;

public partial class StellarFederation
{
    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise<GetListingsResponse> GetListings(string storeId)
    {
        return await Provider.GetService<GetListingsEndpoint>().GetListings(storeId);
    }

    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise UpdateCurrency(string currencyContentId, int amount)
    {
        var invService = Services.Inventory;
        await invService.AddCurrency(currencyContentId, amount);
        BeamableLogger.Log($"Added {amount} of {currencyContentId} to inventory");
    }

    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise AddItem(string itemContentId, Dictionary<string, string>? properties = null)
    {
        var invService = Services.Inventory;
        await invService.AddItem(itemContentId, properties);
        BeamableLogger.Log($"Added {itemContentId} to inventory");
    }

    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise RemoveItem(string itemContentId, long instanceId)
    {
        var invService = Services.Inventory;
        await invService.DeleteItem(itemContentId, instanceId);
        BeamableLogger.Log($"Removed {itemContentId} from inventory");
    }

    /// <summary>
    /// A list of dictionaries with instanceId and properties
    /// </summary>
    /// <param name="items"></param>
    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise UpdateItems(List<CropUpdateRequest> items)
    {
        try
        {
            var invService = Services.Inventory;
            var builder = new InventoryUpdateBuilder();
            foreach (var item in items)
            {
                builder.UpdateItem(item.ContentId, item.InstanceId, item.Properties);
            }

            await invService.Update(builder);
        }
        catch (Exception e)
        {
            BeamableLogger.Log($"Error updating items: {e.Message}");
        }
    }
    
    
    /// <summary>
    /// Updates inventory for a specific currency
    /// </summary>
    /// <param name="currencyContentId"></param>
    /// <param name="amount"></param>
    /// <param name="items"></param>
    [ClientCallable(""), SwaggerCategory("Commerce")]
    public async Promise UpdateInventory(string currencyContentId, int amount, List<CropUpdateRequest> items)
    {
        try
        {
            var invService = Services.Inventory;
            var builder = new InventoryUpdateBuilder();
            foreach (var item in items)
            {
                builder.UpdateItem(item.ContentId, item.InstanceId, item.Properties);
            }
            builder.CurrencyChange(currencyContentId, amount);
            await invService.Update(builder);
            BeamableLogger.Log($"Updated inventory for {currencyContentId}");
        }
        catch (Exception e)
        {
            BeamableLogger.Log($"Error updating items: {e.Message}");
        }
    }
}