using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Contract.Exceptions;
using Beamable.StellarFederation.Features.Contract.Functions;
using Beamable.StellarFederation.Features.Stellar.Exceptions;
using Beamable.StellarFederation.Features.Stellar.Extensions;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarRpcClient : IService
{
    private readonly Configuration _configuration;
    private readonly StellarService _stellarService;
    private readonly AccountsService _accountsService;
    private readonly TransactionManager _transactionManager;
    private readonly TransactionBatchService _transactionBatchService;

    private const int MaxRetries = 5;
    private const int InitialDelayMs = 1000;
    private int _retryCount;

    public StellarRpcClient(Configuration configuration, StellarService stellarService, AccountsService accountsService, TransactionManager transactionManager, TransactionBatchService transactionBatchService)
    {
        _configuration = configuration;
        _stellarService = stellarService;
        _accountsService = accountsService;
        _transactionManager = transactionManager;
        _transactionBatchService = transactionBatchService;
    }

    public async Task<string> SendTransactionAsync<TContractMessage>(string contractAddress, TContractMessage functionMessage) where TContractMessage : IFunctionMessage
    {
        while (true)
        {
            try
            {
                return await SendTransactionInternalAsync(contractAddress, functionMessage);
            }
            catch (StellarTransactionException ex)
            {
                _retryCount++;
                if (_retryCount >= MaxRetries)
                {
                    throw new SendTransactionException($"Maximum retry count exceeded for transaction. Error: {ex.Message}");
                }

                var delayMs = (int)Math.Pow(2, _retryCount - 1) * InitialDelayMs;
                BeamableLogger.LogWarning("Retrying [{retry}] transaction in {N} milliseconds. Error: {error}", _retryCount, delayMs, ex.ToLogFormat());
                await Task.Delay(delayMs);
            }
        }
    }

    private async Task<string> SendTransactionInternalAsync<TContractMessage>(string contractAddress, TContractMessage functionMessage) where TContractMessage : IFunctionMessage
    {
        using (new Measure(functionMessage.FunctionName))
        {
            var transactionHash = string.Empty;
            try
            {
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                var issuerAccount = await _stellarService.GetRealmStellarAccount(realmAccount.Address);

                var invokeContractOperation = new InvokeContractOperation(contractAddress, functionMessage.FunctionName, functionMessage.ToArgs(), realmAccount.KeyPair);
                var transaction = new TransactionBuilder(issuerAccount)
                    .AddOperation(invokeContractOperation)
                    .Build();
                var simulateResponse = await _stellarService.SimulateTransaction(transaction);

                if (simulateResponse.SorobanTransactionData is null || simulateResponse.SorobanAuthorization is null ||
                    simulateResponse.MinResourceFee is null)
                {
                    throw new SendTransactionException($"Simulate transaction failed for contract {contractAddress}, function {functionMessage.FunctionName}. Error: {simulateResponse.Error}");
                }

                transaction.SetSorobanTransactionData(simulateResponse.SorobanTransactionData);
                transaction.SetSorobanAuthorization(simulateResponse.SorobanAuthorization);

                var minResourceFee = simulateResponse.MinResourceFee ?? 0;
                var buffer = (uint)Math.Max(minResourceFee * (await _configuration.ExtraResourceFeePercentage/100), await _configuration.MinExtraResourceFeeInStroops);
                transaction.AddResourceFee(minResourceFee + buffer);
                transaction.Sign(realmAccount.KeyPair);
                var sendResponse = await _stellarService.SendTransaction(transaction);
                var result = sendResponse.ToStellarTransactionResult();
                if (result.ShouldRetry())
                {
                    throw new StellarTransactionException(result.Error.Message);
                }

                transactionHash = result.Hash;

                await _transactionManager.AddChainTransaction(functionMessage.TransactionId, new ChainTransaction
                {
                    Data = JsonSerializer.Serialize(functionMessage),
                    Function = functionMessage.FunctionName,
                    Hash = transactionHash
                });
            }
            catch (Exception ex)
            {
                await _transactionManager.AddChainTransaction(functionMessage.TransactionId, new ChainTransaction
                {
                    Function = nameof(functionMessage),
                    Data = JsonSerializer.Serialize(functionMessage),
                    Error = ex.Message
                });

                BeamableLogger.LogError("Transaction for contract {N} failed with error: {e}", contractAddress, ex.ToLogFormat());
            }
            return transactionHash;
        }
    }

    public async Task<string> SendNativeTransactionAsync<TFunctionNativeMessage>(Transaction transaction, TFunctionNativeMessage[] functionMessages) where TFunctionNativeMessage : IFunctionNativeMessage
    {
        while (true)
        {
            try
            {
                return await SendNativeTransactionInternalAsync(transaction, functionMessages);
            }
            catch (StellarTransactionException ex)
            {
                _retryCount++;
                if (_retryCount >= MaxRetries)
                {
                    throw new SendTransactionException($"Maximum retry count exceeded for transaction. Error: {ex.Message}");
                }

                var delayMs = (int)Math.Pow(2, _retryCount - 1) * InitialDelayMs;
                BeamableLogger.LogWarning("Retrying [{retry}] transaction in {N} milliseconds. Error: {error}", _retryCount, delayMs, ex.ToLogFormat());
                await Task.Delay(delayMs);
            }
        }
    }

    private async Task<string> SendNativeTransactionInternalAsync<TFunctionNativeMessage>(Transaction transaction, TFunctionNativeMessage[] functionMessages) where TFunctionNativeMessage : IFunctionNativeMessage
    {
        using (new Measure(functionMessages.First().FunctionName))
        {
            var transactionHash = string.Empty;
            try
            {
                var sendResponse = await _stellarService.SendTransaction(transaction);
                var result = sendResponse.ToStellarTransactionResult();
                if (result.ShouldRetry())
                {
                    throw new StellarTransactionException(result.Error.Message);
                }

                transactionHash = result.Hash;

                await _transactionManager.AddChainTransactions(functionMessages, transactionHash);
                await _transactionBatchService.UpdateStatus(functionMessages.Select(x => x.TransactionId).ToArray(),
                    TransactionStatus.Sent);
            }
            catch (Exception ex)
            {
                await _transactionManager.AddChainTransactions(functionMessages, transactionHash, ex.Message);
                await _transactionBatchService.UpdateStatus(functionMessages.Select(x => x.TransactionId).ToArray(),
                    TransactionStatus.Failed);
                BeamableLogger.LogError("Transaction for native function {N} failed with error: {e}", functionMessages.First().FunctionName, ex.ToLogFormat());
            }
            return transactionHash;
        }
    }
}