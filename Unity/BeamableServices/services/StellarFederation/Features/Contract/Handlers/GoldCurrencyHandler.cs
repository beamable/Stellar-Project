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

public class GoldCurrencyHandler : IService, IContentContractHandler
{
    private readonly ContractService _contractService;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;
    private readonly AccountsService _accountsService;

    public GoldCurrencyHandler(ContractService contractService, StellarService stellarService, CliClient cliClient, AccountsService accountsService)
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
            var contract = await _contractService.GetByContent<GoldContract>(model.ContentObject.Id);
            if (contract != null)
            {
                var objectExists = await _stellarService.ContractExists(contract.Address);
                if (objectExists)
                {
                    BeamableLogger.Log($"Contract for {model.ContentObject.Id} already exists.");
                    return;
                }
            }

            if (model.ContentObject is not GoldCurrency coinCurrency)
                throw new ContractException($"{model.ContentObject.Id} is not a {nameof(GoldCurrency)}");

            BeamableLogger.Log($"Creating contract for {model.ContentObject.Id}...");
            var moduleName = coinCurrency.ToCurrencyModuleName();
            var contractAccount = await _accountsService.GetAccount(model.ContentObject.ToContractAccountName());
            if (contractAccount is null)
                throw new ContractException($"Account for {model.ContentObject.ToContractAccountName()} is not created.");
            await _cliClient.CreateProject(moduleName);
            await WriteContractTemplate(coinCurrency);
            await _cliClient.CopyContractCode(moduleName);
            await _cliClient.CompileContract(moduleName);
            var contractAddress = await _cliClient.DeployContract(moduleName, contractAccount.Value);
            await _contractService.UpsertContract(new GoldContract
            {
                ContentId = model.ContentObject.Id,
                Address = contractAddress.Trim(),
                TotalSupply = coinCurrency.TotalSupply.ToString()
            }, model.ContentObject.Id);
            BeamableLogger.Log($"Created contract for {coinCurrency.Id} with address {contractAddress}");
        }
        catch (Exception e)
        {
            throw new ContractException($"Error in creating contract for {model.ContentObject.Id}, exception: {e.Message}");
        }
    }

    private async Task WriteContractTemplate(GoldCurrency coinCurrency)
    {
        var itemTemplate = await File.ReadAllTextAsync("Features/Contract/Templates/gold.rs");
        var template = Handlebars.Compile(itemTemplate);
        var itemResult = template(coinCurrency);
        var contractPath = $"{CliClient.ContractSourcePath}/{coinCurrency.ToCurrencyModuleName()}.rc";
        await ContractWriter.WriteContract(contractPath, itemResult);
    }
}