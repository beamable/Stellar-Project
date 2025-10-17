using System;
using System.IO;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Exceptions;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;
using HandlebarsDotNet;
using StellarFederationCommon.Extensions;
using StellarFederationCommon.FederationContent;

namespace Beamable.StellarFederation.Features.Contract.Handlers;

public class CoinCurrencyHandler : IService, IContentContractHandler
{
    private readonly ContractService _contractService;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;
    private readonly AccountsService _accountsService;

    public CoinCurrencyHandler(ContractService contractService, StellarService stellarService, CliClient cliClient, AccountsService accountsService)
    {
        _contractService = contractService;
        _stellarService = stellarService;
        _cliClient = cliClient;
        _accountsService = accountsService;
    }

    public async Task HandleContract(ContentContractsModel model)
    {
        try
        {
            var contract = await _contractService.GetByContent<CoinContract>(model.ContentObject.Id);
            if (contract != null)
            {
                var objectExists = await _stellarService.ContractExists(contract.Address);
                if (objectExists)
                {
                    BeamableLogger.Log($"Contract for {model.ContentObject.Id} already exists.");
                    return;
                }
            }

            if (model.ContentObject is not CoinCurrency coinCurrency)
                throw new ContractException($"{model.ContentObject.Id} is not a {nameof(CoinCurrency)}");

            BeamableLogger.Log($"Creating contract for {model.ContentObject.Id}...");
            var realmAccount = await _accountsService.GetOrCreateRealmAccount();
            await _cliClient.CreateProject(coinCurrency.ToModuleName());
            await WriteContractTemplate(coinCurrency);
            await _cliClient.CopyContractCode(coinCurrency.ToModuleName());
            await _cliClient.CompileContract(coinCurrency.ToModuleName());
            var contractAddress = await _cliClient.DeployContract(coinCurrency.ToModuleName(), realmAccount);
            await _contractService.UpsertContract(new CoinContract
            {
                ContentId = model.ContentObject.Id,
                Address = contractAddress.Trim()
            }, model.ContentObject.Id);
            BeamableLogger.Log($"Created contract for {coinCurrency.Id}");
        }
        catch (Exception e)
        {
            throw new ContractException($"Error in creating contract for {model.ContentObject.Id}, exception: {e.Message}");
        }
    }

    private async Task WriteContractTemplate(CoinCurrency coinCurrency)
    {
        var itemTemplate = await File.ReadAllTextAsync("Features/Contract/Templates/coin.rs");
        var template = Handlebars.Compile(itemTemplate);
        var itemResult = template(coinCurrency);
        var contractPath = $"{CliClient.ContractSourcePath}/{coinCurrency.ToModuleName()}.rc";
        await ContractWriter.WriteContract(contractPath, itemResult);
    }
}