using System;
using Beamable.StellarFederation.Features.Common;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Responses.Results;
using StellarDotnetSdk.Responses.SorobanRpc;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.Stellar;

public static class StellarServiceExtensions
{
    private const uint LedgersPerDay = 17_280; //5 sec per ledger
    public static uint ExpiresInDays(uint currentLedger, uint days)
        => currentLedger + (LedgersPerDay * days);

    public static TransactionBuilder AddCreateAccountOperation(this TransactionBuilder builder, KeyPair keyPair, StellarAmount startingAmount)
    {
        builder.AddOperation(new CreateAccountOperation(
            keyPair,
            startingAmount.ToXlmString()));
        return builder;
    }

    public static TransactionBuilder AddNativeTransferOperation(this TransactionBuilder builder, KeyPair keyPair, StellarAmount startingAmount)
    {
        builder.AddOperation(new PaymentOperation(
            keyPair,
            new AssetTypeNative(),
            startingAmount.ToXlmString()));
        return builder;
    }

    public static TransactionBuilder Defaults(this TransactionBuilder builder, uint baseFee, int timeoutSec, string memo)
    {
        builder.SetFee(baseFee)
            .AddMemo(Memo.Text(memo))
            .AddTimeBounds(GetDefaultTimeBounds(timeoutSec));
        return builder;
    }

    private static TimeBounds GetDefaultTimeBounds(int timeoutSec)
    {
        return new TimeBounds(0, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + timeoutSec);
    }

    public static TransactionBuilder AddBeginSponsoringFutureReservesOperation(this TransactionBuilder builder, KeyPair keyPair)
    {
        builder.AddOperation(new BeginSponsoringFutureReservesOperation(
            keyPair));
        return builder;
    }

    public static TransactionBuilder AddEndSponsoringFutureReservesOperation(this TransactionBuilder builder, KeyPair keyPair)
    {
        builder.AddOperation(new EndSponsoringFutureReservesOperation(
            keyPair));
        return builder;
    }

    public static TransactionBuilder AddCloseAccountOperation(this TransactionBuilder builder, KeyPair destination, KeyPair source)
    {
        builder.AddOperation(new AccountMergeOperation(
            destination, source));
        return builder;
    }

    public static string TransactionError(this SendTransactionResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.ErrorResultXdr))
            return string.Empty;
        var result = TransactionResult.FromXdrBase64(response.ErrorResultXdr);
        return "";
    }

    public static string TransactionError(this GetTransactionResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.ResultXdr))
            return string.Empty;
        var resultBase = TransactionResult.FromXdrBase64(response.ResultXdr);
        // if (result is TransactionResultFailed resultFailed)
        // {
        //     foreach (var message in resultFailed.Results)
        //     {
        //         var i = 0;
        //     }
        // }
        return "";
    }
}