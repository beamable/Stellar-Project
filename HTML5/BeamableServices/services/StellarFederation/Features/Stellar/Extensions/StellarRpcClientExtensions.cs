using System;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Responses.SorobanRpc;
using StellarDotnetSdk.Xdr;

namespace Beamable.StellarFederation.Features.Stellar.Extensions;

public static class StellarRpcClientExtensions
{
    public static StellarTransactionResult ToStellarTransactionResult(this SendTransactionResponse response)
    {
        return new StellarTransactionResult(response.ToStellarStatus(), response.Hash, response.DecodeErrorResultXdr());
    }

    private static StellarTransactionStatus ToStellarStatus(this SendTransactionResponse sendStatus)
    {
        return sendStatus.Status == SendTransactionResponse.SendTransactionStatus.PENDING
            ? StellarTransactionStatus.Accepted
            : StellarTransactionStatus.Failed;
    }

    private static StellarTransactionError DecodeErrorResultXdr(this SendTransactionResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.ErrorResultXdr))
            return StellarTransactionError.Empty();

        var reader = new XdrDataInputStream(Convert.FromBase64String(response.ErrorResultXdr));
        var transactionResult = TransactionResult.Decode(reader);
        return new StellarTransactionError(transactionResult.Result.Discriminant.InnerValue.ToString(), transactionResult.Result.Discriminant.InnerValue);
    }

    public static bool ShouldRetry(this StellarTransactionResult result)
    {
        return result.Status != StellarTransactionStatus.Accepted;
    }

    public static string ToAddressFromAuth(this StellarDotnetSdk.Operations.SorobanAuthorizationEntry entry)
    {
        var scAddress = entry.Credentials.ToXdr();
        var addressToSign = scAddress.Address.Address;
        var publicKeyBytes = addressToSign.AccountId.InnerValue.Ed25519.InnerValue;
        var userKeyPair = KeyPair.FromPublicKey(publicKeyBytes);
        return userKeyPair.Address;
    }
}