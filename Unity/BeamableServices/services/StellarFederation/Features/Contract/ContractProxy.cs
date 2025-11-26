using System;
using System.Threading.Tasks;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract.Exceptions;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy : IService
{
    private readonly ContractService _contractService;
    private readonly StellarRpcClient _stellarRpcClient;
    private readonly StellarService _stellarService;
    private readonly AccountsService _accountsService;

    public ContractProxy(ContractService contractService, StellarRpcClient stellarRpcClient, AccountsService accountsService, StellarService stellarService)
    {
        _contractService = contractService;
        _stellarRpcClient = stellarRpcClient;
        _accountsService = accountsService;
        _stellarService = stellarService;
    }

    public async Task<TContract> GetContract<TContract>(string contentId) where TContract : ContractBase
    {
        return await GlobalCache.GetOrCreate<TContract>(contentId, async _ => await _contractService.GetByContentId<TContract>(contentId), TimeSpan.FromDays(1)) ?? throw new ContractException($"Contract for {contentId} don't exist.");;
    }
}