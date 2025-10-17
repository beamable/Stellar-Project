using System;
using System.Collections.Generic;
using StellarDotnetSdk.Responses.Results;
using StellarDotnetSdk.Responses.SorobanRpc;

namespace Beamable.StellarFederation.Features.Stellar;

public static class XdrErrorDecoder
{
    public static string GetErrorMessage(this SendTransactionResponse response)
    {
        var errorMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(response.ErrorResultXdr))
        {
            return string.Empty;
        }

        try
        {
            var txResult = TransactionResult.FromXdrBase64(response.ErrorResultXdr);
            switch (txResult)
            {
                case TransactionResultFailed failedTxResult:
                    foreach (var opResult in failedTxResult.Results)
                    {
                        if (opResult.IsSuccess) continue;
                        errorMessages.Add($"Operation Failed with: {opResult.GetType().Name}");
                    }
                    break;
                default:
                    errorMessages.Add($"Transaction rejected with: {txResult.GetType().Name}");
                    break;
            }
        }
        catch (Exception ex)
        {
            return $"FATAL: Failed to parse XDR string. Error: {ex.Message}";
        }

        return string.Join(",", errorMessages);
    }

    public static string GetErrorMessage(this GetTransactionResponse response)
    {
        var errorMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(response.ResultXdr))
        {
            return response.Status.ToString();
        }

        try
        {
            var txResult = TransactionResult.FromXdrBase64(response.ResultXdr);
            switch (txResult)
            {
                case TransactionResultFailed failedTxResult:
                    foreach (var opResult in failedTxResult.Results)
                    {
                        if (opResult.IsSuccess) continue;
                        errorMessages.Add($"Operation Failed with: {opResult.GetType().Name}");
                    }
                    break;
                default:
                    errorMessages.Add($"Transaction rejected with: {txResult.GetType().Name}");
                    break;
            }
        }
        catch (Exception ex)
        {
            return $"FATAL: Failed to parse XDR string. Error: {ex.Message}";
        }

        return string.Join(",", errorMessages);
    }
}