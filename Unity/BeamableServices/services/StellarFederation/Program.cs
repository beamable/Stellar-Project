using System;
using System.Reflection;
using Beamable.Server;
using System.Threading.Tasks;
using StellarFederationCommon;

namespace Beamable.StellarFederation
{
	public class Program
	{
		/// <summary>
		/// The entry point for the <see cref="StellarFederation"/> service.
		/// </summary>
		public static async Task Main()
		{
			//Preload content types from Common project
			AppDomain.CurrentDomain.Load(Assembly.GetAssembly(typeof(StellarFederationCommonAssemblyIdentifier))!.GetName());

			// inject data from the CLI.
			await MicroserviceBootstrapper.Prepare<StellarFederation>();
			
			// run the Microservice code
			await MicroserviceBootstrapper.Start<StellarFederation>();
		}
	}
}