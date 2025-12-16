using System;
using System.IO;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Inventory;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Exceptions;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Minting;
using Beamable.StellarFederation.Features.Stellar;
using HandlebarsDotNet;
using StellarFederationCommon.Extensions;
using StellarFederationCommon.FederationContent;

namespace Beamable.StellarFederation.Features.Contract.Handlers;

public class ItemHandler : IService, IContentContractHandler
{
    private readonly ContractService _contractService;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;
    private readonly AccountsService _accountsService;
    private readonly MetadataService _metadataService;


    public ItemHandler(ContractService contractService, StellarService stellarService, CliClient cliClient, AccountsService accountsService, MetadataService metadataService)
    {
        _contractService = contractService;
        _stellarService = stellarService;
        _cliClient = cliClient;
        _accountsService = accountsService;
        _metadataService = metadataService;
    }

    async Task IContentContractHandler.HandleContract(ContentContractsModel model)
    {
        try
        {
            var itemName = model.ContentObject.ToItemNameType();
            var contract = await _contractService.GetByContent<ItemContract>(itemName);
            if (contract != null)
            {
                var objectExists = await _stellarService.ContractExists(contract.Address);
                if (objectExists)
                {
                    BeamableLogger.Log($"Contract for {itemName} already exists.");
                    return;
                }
            }

            if (model.ContentObject is not ItemContent itemContent)
                throw new ContractException($"{model.ContentObject.Id} is not a {nameof(ItemContent)}");

            var moduleName = model.Key.ToItemNameType();
            BeamableLogger.Log($"Creating contract for {moduleName}...");
            var contractAccount = await _accountsService.GetAccount(itemName);
            if (contractAccount is null)
                throw new ContractException($"Account for {itemName} is not created.");

            var baseUri = await _metadataService.GetBaseUri(moduleName);
            await _cliClient.CreateProject(moduleName);
            await WriteContractTemplate(moduleName, baseUri);
            await _cliClient.CopyContractCode(moduleName);
            await _cliClient.CompileContract(moduleName);
            var contractAddress = await _cliClient.DeployContract(moduleName, contractAccount.Value);


             await _contractService.UpsertContract(new ItemContract
             {
                 ContentId = itemName,
                 Address = contractAddress.Trim(),
                 BaseUri = baseUri
             }, itemName);

            BeamableLogger.Log($"Created contract for {moduleName} with address {contractAddress}");
        }
        catch (Exception e)
        {
            throw new ContractException($"Error in creating contract for {model.ContentObject.ToItemNameType()}, exception: {e.Message}");
        }
    }

    private async Task WriteContractTemplate(string moduleName, string baseUri)
    {
        var model = new ItemContractModel(moduleName,  baseUri);
        var itemTemplate = await File.ReadAllTextAsync("Features/Contract/Templates/nft.rs");
        var template = Handlebars.Compile(itemTemplate);
        var itemResult = template(model);
        var contractPath = $"{CliClient.ContractSourcePath}/{moduleName}.rc";
        await ContractWriter.WriteContract(contractPath, itemResult);
    }

    private record ItemContractModel(string Name, string BaseUri);
}