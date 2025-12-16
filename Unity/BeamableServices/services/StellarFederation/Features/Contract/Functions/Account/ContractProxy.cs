using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Stellar;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<Transaction> PrepareCreateAccountWithSponsorship(CreateAccountFunctionMessage[] functionMessages)
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var sourceAccount = await _stellarService.GetRealmStellarAccount(realmAccount.Address);
        var transactionBuilder = await _stellarService.CreateDefaultNativeBuilder(sourceAccount, functionMessages.First().FunctionName);

        foreach (var functionMessage in functionMessages)
        {
            transactionBuilder
                .AddBeginSponsoringFutureReservesOperation(functionMessage.NewAccount.KeyPair);
            transactionBuilder
                .AddCreateAccountOperation(functionMessage.NewAccount.KeyPair, StellarAmount.NativeZero);
            transactionBuilder
                .AddEndSponsoringFutureReservesOperation(functionMessage.NewAccount.KeyPair);
        }
        var transaction = transactionBuilder.Build();
        transaction.Sign(realmAccount.KeyPair);
        return transaction;
    }

    public async Task<string> SendCreateAccountWithSponsorship(Transaction transaction, CreateAccountFunctionMessage[] functionMessages)
    {
        return await _stellarRpcClient.SendNativeTransactionAsync(transaction, functionMessages);
    }

    public async Task<Transaction> PrepareCloseAccountWithSponsorship(CloseAccountFunctionMessage[] functionMessages)
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var sourceAccount = await _stellarService.GetRealmStellarAccount(realmAccount.Address);
        var transactionBuilder = await _stellarService.CreateDefaultNativeBuilder(sourceAccount, functionMessages.First().FunctionName);

        foreach (var functionMessage in functionMessages)
        {
            transactionBuilder
                .AddCloseAccountOperation(realmAccount.KeyPair, functionMessage.Account.KeyPair);
        }
        var transaction = transactionBuilder.Build();
        transaction.Sign(realmAccount.KeyPair);
        return transaction;
    }

    public async Task<string> SendCloseAccountWithSponsorship(Transaction transaction, CloseAccountFunctionMessage[] functionMessages)
    {
        return await _stellarRpcClient.SendNativeTransactionAsync(transaction, functionMessages);
    }
}