using Beamable.Common;

namespace StellarFederationCommon
{
	/// <summary>
	/// SuiWeb3Identity definition
	/// </summary>
	[FederationId(StellarFederationSettings.StellarIdentityName)]
	public class StellarWeb3Identity : IFederationId {}

	/// <summary>
	/// SuiWeb3ExternalIdentity definition
	/// </summary>
	[FederationId(StellarFederationSettings.StellarExternalIdentityName)]
	public class StellarWeb3ExternalIdentity : IFederationId {}


	/// <summary>
	/// StellarFederationSettings class
	/// </summary>
	public static class StellarFederationSettings
	{
		///<Summary>
		/// StellarFederation microservice name
		///</Summary>
		public const string MicroserviceName = "StellarFederation";
		///<Summary>
		/// StellarFederationIdentity name
		///</Summary>
		public const string StellarIdentityName = "StellarIdentity";
		///<Summary>
		/// StellarFederationIdentity name
		///</Summary>
		public const string StellarExternalIdentityName = "StellarExternalIdentity";
	}



	/// <summary>
	/// Empty type used for StellarFederationCommon assembly load in the Federation service
	/// </summary>
	public class StellarFederationCommonAssemblyIdentifier {}
}