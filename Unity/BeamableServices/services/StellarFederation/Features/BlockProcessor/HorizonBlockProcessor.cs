using System;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.BlockProcessor.Handlers;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Scheduler.Storage;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Stellar.Models;

namespace Beamable.StellarFederation.Features.BlockProcessor;

public class HorizonBlockProcessor(
    StellarService stellarService,
    AccountsService accountsService,
    BlockCollection blockCollection,
    Configuration configuration,
    CreateAccountBlockHandler accountBlockHandler) : IService
{

    public async Task Handle()
    {
        var fromBlockModel = await blockCollection.Get(await configuration.StellarHorizon, StellarSettings.HorizonApi);
        if (fromBlockModel is null)
            return;
        try
        {
            var realmAccount = await accountsService.GetOrCreateRealmAccount();
            var logs = await stellarService.GetHorizonLogs(realmAccount.Address, fromBlockModel);
            fromBlockModel.BlockNumber = logs.LastProcessedLedger;
            fromBlockModel.Cursor = logs.LastCursor;
            await HandleAccounts(logs);
        }
        catch (Exception e)
        {
            BeamableLogger.LogWarning("Exception in HorizonBlockProcessor", e.ToLogFormat());
        }
        finally
        {
            await blockCollection.InsertBlock(fromBlockModel);
        }
    }

    private async Task HandleAccounts(HorizonLogsResponse logs)
    {
        var accountLogs = logs.Transactions.Where(r => r.MemoValue is not null && r.MemoValue == CreateAccountFunctionMessage.StaticFunctionName);
        await accountBlockHandler.Handle(accountLogs);
    }
}