using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.BackgroundService;
using Beamable.StellarFederation.Endpoints;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Scheduler;
using StellarFederationCommon;
using StellarFederationCommon.FederationContent;
using StellarFederationCommon.Models.Response;

namespace Beamable.StellarFederation
{
	[Microservice(StellarFederationSettings.MicroserviceName)]
	public partial class StellarFederation : Microservice, IFederatedInventory<StellarWeb3Identity>
	{
		[ConfigureServices]
		public static void Configure(IServiceBuilder serviceBuilder)
		{
			var dependencyBuilder = serviceBuilder.Builder;
			dependencyBuilder.AddFeatures();
			dependencyBuilder.AddEndpoints();
		}

		[InitializeServices]
		public static async Task Initialize(IServiceInitializer initializer)
		{
			try
			{
				initializer.SetupExtensions();

				// Set provider for background service
				BackgroundServiceState.SetMicroserviceProvider(initializer.Provider);
				BackgroundServiceState.Initialized = true;

				// Validate configuration
				if (string.IsNullOrWhiteSpace(await initializer.Provider.GetService<Configuration>().StellarRpc))
				{
					throw new ConfigurationException($"{nameof(Configuration.StellarRpc)} is not defined in realm config. Please apply the configuration and restart the service to make it operational.");
				}

				//Generate Realm account
				await initializer.Provider.GetService<AccountsService>().GetOrCreateRealmAccount();

				// Initialize Contracts
#if !DEBUG
				await initializer.GetService<Features.Contract.ContractService>().InitializeContentContracts();
				await initializer.Provider.GetService<SchedulerService>().Start();
#endif
			}
			catch (Exception ex)
			{
				BeamableLogger.LogException(ex);
				BeamableLogger.LogError("Service initialization failed. Fix the issues before using the service.");
			}
		}

		[AdminOnlyCallable]
		public async Promise<string> GetRealmAccount()
		{
			var account = await Provider.GetService<AccountsService>()
				.GetRealmAccount();
			return account?.Address ?? "";
		}

		[AdminOnlyCallable]
		public async Promise<string> GenerateRealmAccount()
		{
			var account = await Provider.GetService<AccountsService>()
				.GetOrCreateRealmAccount();
			return account.Address;
		}

		async Promise<FederatedAuthenticationResponse> IFederatedLogin<StellarWeb3Identity>.Authenticate(string token, string challenge, string solution)
		{
			return await Provider.GetService<AuthenticateEndpoint>()
				.Authenticate(token, challenge, solution);
		}

		async Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3Identity>.GetInventoryState(string id)
		{
			var microserviceInfo = MicroserviceMetadataExtensions.GetMetadata<StellarFederation, StellarWeb3Identity>();
			return await Provider.GetService<GetInventoryStateEndpoint>()
				.GetInventoryState(id, microserviceInfo);
		}

		async Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3Identity>.StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
			List<FederatedItemUpdateRequest> updateItems)
		{
			var gamerTag = await Provider.GetService<AccountsService>().GetGamerTag(id);
			var microserviceInfo = MicroserviceMetadataExtensions.GetMetadata<StellarFederation, StellarWeb3Identity>();
			return await Provider.GetService<StartInventoryTransactionEndpoint>()
				.StartInventoryTransaction(id, transaction, currencies, newItems, deleteItems, updateItems, gamerTag, microserviceInfo);
		}

		[ClientCallable]
		public async Promise<ConfigurationResponse> StellarConfiguration()
		{
			var configuration = Provider.GetService<Configuration>();
			return new ConfigurationResponse
			{
				network = await configuration.StellarNetwork,
				walletConnectBridgeUrl = await configuration.WalletConnectBridgeUrl
			};
		}

		[ClientCallable]
		public async Promise<AccountResponse> CreateAccount()
		{
			var account = await Provider.GetService<AccountsService>().CreateNewAccount(Context.UserId.ToString());
			return new AccountResponse
			{
				wallet = account.HasValue ? account.Value.Address : "",
				created = account?.Created ?? false
			};
		}

		[ClientCallable]
		public async Promise<AccountResponse> GetAccount(string id)
		{
			//var account = await Provider.GetService<AccountsService>().GetAccount(Context.UserId.ToString());
			var account = await Provider.GetService<AccountsService>().GetAccount(id);
			return new AccountResponse
			{
				wallet = account.HasValue ? account.Value.Address : "",
				created = account?.Created ?? false
			};
		}

		[ClientCallable]
		public async Promise Test()
		{
			await Provider.GetService<TestService>().Test();
		}

		[ClientCallable]
		public async Promise<string> Test2()
		{
			return await Provider.GetService<TestService>().Test2();
		}

		#region Placehorders for moe: to be removed later and replaced with proper endpoints in game

		[ClientCallable]
		public async Promise UpdateCurrency(string currencyContentId, int amount)
		{
			var invService = Services.Inventory;
			await invService.AddCurrency(currencyContentId, amount);
			BeamableLogger.Log($"Added {amount} of {currencyContentId} to inventory");
		}


		[ClientCallable]
		public async Promise AddItem(string itemContentId, Dictionary<string, string>? properties = null)
		{
			var invService = Services.Inventory;
			await invService.AddItem(itemContentId, properties);
			BeamableLogger.Log($"Added {itemContentId} to inventory");
		}

		[ClientCallable]
		public async Promise RemoveItem(string itemContentId, long instanceId)
		{
			var invService = Services.Inventory;
			await invService.DeleteItem(itemContentId, instanceId);
			BeamableLogger.Log($"Removed {itemContentId} from inventory");
		}

		/// <summary>
		/// A list of dictionaries with instanceId and properties
		/// </summary>
		/// <param name="items"></param>
		[ClientCallable]
		public async Promise UpdateItems(List<CropUpdateRequest> items)
		{
			try
			{
				var invService = Services.Inventory;
				var builder = new InventoryUpdateBuilder();
				foreach (var item in items)
				{
						builder.UpdateItem(item.ContentId, item.InstanceId, item.Properties);
				}

				await invService.Update(builder);
			}
			catch (Exception e)
			{
				BeamableLogger.Log($"Error updating items: {e.Message}");
			}
		}
		#endregion
	}
}