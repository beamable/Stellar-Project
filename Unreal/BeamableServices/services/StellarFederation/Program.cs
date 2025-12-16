using System;
using System.Collections.Generic;
using System.Reflection;
using Beamable.Server;
using System.Threading.Tasks;
using Beamable.StellarFederation.BackgroundService;
using Beamable.StellarFederation.Features.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StellarFederationCommon;
using ZLogger;

namespace Beamable.StellarFederation
{
	public class Program
	{
		/// <summary>
		/// The entry point for the <see cref="StellarFederation"/> service.
		/// </summary>
		public static async Task Main(string[] args)
		{
			//Preload content types from Common project
			AppDomain.CurrentDomain.Load(Assembly.GetAssembly(typeof(StellarFederationCommonAssemblyIdentifier))!.GetName());

			// inject data from the CLI.
			await MicroserviceBootstrapper.Prepare<StellarFederation>();

			// Registering in-flight transaction shutdown hook.
			TransactionManager.SetupShutdownHook();

			// run the Microservice code
			var tasks = CreateMicroserviceTasks(args);
			await Task.WhenAll(tasks);
		}

		private static List<Task> CreateMicroserviceTasks(string[] args)
		{
			// run the Microservice code
			var list = new List<Task> { MicroserviceBootstrapper.Start<StellarFederation>() };
			//OPENAPI generation adds CLArgs --generate-oapi, don't run the hosted service in that case
			Console.WriteLine("Command-line arguments: " + string.Join(", ", args));
			if (args.Length == 0)
			{
				var builder = Host.CreateApplicationBuilder();
				builder.Services.AddBackgroundServiceFeatures();
				builder.Services.AddSingleton<StellarBackgroundService>();
				builder.Services.AddHostedService(provider => provider.GetRequiredService<StellarBackgroundService>());
				builder.Logging.ClearProviders();
				builder.Logging.AddZLoggerConsole();
				BeamableZLoggerProvider.SetLogger(MicroserviceBootstrapper._logger);

				var host = builder.Build();
				BackgroundServiceState.SetHostedProvider(host.Services);
				var hostedTask = host.RunAsync();
				list.Add(hostedTask);
			}
			return list;
		}
	}
}