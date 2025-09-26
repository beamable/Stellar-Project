using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Endpoints;
using Beamable.StellarFederation.Extensions;
using StellarFederationCommon;

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
			}
			catch (Exception ex)
			{
				BeamableLogger.LogException(ex);
				BeamableLogger.LogError("Service initialization failed. Fix the issues before using the service.");
			}
		}

		async Promise<FederatedAuthenticationResponse> IFederatedLogin<StellarWeb3Identity>.Authenticate(string token, string challenge, string solution)
		{
			return await Provider.GetService<AuthenticateEndpoint>()
				.Authenticate(token, challenge, solution);
		}

		Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3Identity>.GetInventoryState(string id)
		{
			throw new NotImplementedException();
		}

		Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3Identity>.StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
			List<FederatedItemUpdateRequest> updateItems)
		{
			throw new NotImplementedException();
		}
	}
}