using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Stellar.Models;

namespace Beamable.StellarFederation.Features.WalletManager;

public class WalletManagerService : IService
{
    private readonly AccountsService _accountsService;
    private readonly StellarService _stellarService;

    public WalletManagerService(AccountsService accountsService, StellarService stellarService)
    {
        _accountsService = accountsService;
        _stellarService = stellarService;
    }

    public async Task CreateContractWallets(List<string> contentKeys)
    {
        var transferBatch = new List<TransferNativeBatch>();
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var realmBalance = await _stellarService.NativeBalance(realmAccount.Address);
        var transferAmount = new StellarAmount(realmBalance * 20 / 100);
        foreach (var contentKey in contentKeys)
        {
            var account = await _accountsService.GetOrCreateAccount(contentKey);
            var nativeBalance = await _stellarService.NativeBalance(account.Address);
            if (nativeBalance > new StellarAmount(50000000))
                continue;

            transferBatch.Add(new TransferNativeBatch
            {
                ToAddress = account.Address,
                Amount = transferAmount
            });
        }
        if (transferBatch.Count > 0)
            await _stellarService.TransferNativeBatch(transferBatch);
    }
}