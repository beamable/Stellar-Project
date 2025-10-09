using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Endpoints;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Stellar;
using StellarFederationCommon;
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

				// Validate configuration
				if (string.IsNullOrWhiteSpace(await initializer.Provider.GetService<Configuration>().StellarRpc))
				{
					throw new ConfigurationException($"{nameof(Configuration.StellarRpc)} is not defined in realm config. Please apply the configuration and restart the service to make it operational.");
				}

				//Generate Realm account
				await initializer.Provider.GetService<AccountsService>().GetOrCreateRealmAccount();
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
			return await Provider.GetService<GetInventoryStateEndpoint>()
				.GetInventoryState(id);
		}

		async Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3Identity>.StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
			List<FederatedItemUpdateRequest> updateItems)
		{
			return await Provider.GetService<StartInventoryTransactionEndpoint>()
				.StartInventoryTransaction(id, transaction, currencies, newItems, deleteItems, updateItems);
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
	}
}