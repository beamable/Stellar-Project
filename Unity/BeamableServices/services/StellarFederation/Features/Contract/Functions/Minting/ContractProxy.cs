using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using StellarDotnetSdk.Xdr;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> CoinBatchMint(MintCoinFunctionMessage request)
    {
        var contract = await GetContract<CoinContract>(request.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, request);
    }

    public async Task<string> ItemBatchMint(MintItemFunctionMessage request)
    {
        var contract = await GetContract<ItemContract>(request.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, request);
    }

    public async Task<string> ItemBatchUpdate(UpdateItemFunctionMessage request)
    {
        var contract = await GetContract<ItemContract>(request.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, request);
    }

    public async Task<List<uint>> GetItemBalance(string wallet, string contentId)
    {
        var contract = await GetContract<ContractBase>(contentId);
        var balanceResponse = await _stellarRpcClient.InvokeContractAsync(contract.Address,
            new ItemBalanceFunctionMessage(contentId,
                wallet));

        var tokenIds = new List<uint>();
        var reader = new XdrDataInputStream(Convert.FromBase64String(balanceResponse));
        var scVal = SCVal.Decode(reader);
        if (scVal.Discriminant.InnerValue == SCValType.SCValTypeEnum.SCV_VEC)
        {
            var vec = scVal.Vec;
            tokenIds.AddRange(from item in vec.InnerValue where item.Discriminant.InnerValue == SCValType.SCValTypeEnum.SCV_U32 select item.U32.InnerValue);
        }
        return tokenIds;
    }
}