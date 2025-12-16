using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Stellar;
using StellarDotnetSdk.Accounts;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> TransferNative(TransferNativeFunctionMessage functionMessage)
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var destinationKeyPair = KeyPair.FromAccountId(functionMessage.To);
        var sourceAccount = await _stellarService.GetRealmStellarAccount(realmAccount.Address);
        var destinationAccount = await _stellarService.GetStellarAccount(functionMessage.To);
        var transactionBuilder = await _stellarService.CreateDefaultNativeBuilder(sourceAccount, functionMessage.FunctionName);
        var transferAmount = new StellarAmount(functionMessage.Amount);
        if (destinationAccount is null)
        {
            transactionBuilder
                .AddCreateAccountOperation(destinationKeyPair, transferAmount);
        }
        else
        {
            transactionBuilder
                .AddNativeTransferOperation(destinationKeyPair, transferAmount);
        }
        var transaction = transactionBuilder.Build();
        transaction.Sign(realmAccount.KeyPair);
        return await _stellarRpcClient.SendNativeTransactionAsync(transaction, [functionMessage]);
    }
}