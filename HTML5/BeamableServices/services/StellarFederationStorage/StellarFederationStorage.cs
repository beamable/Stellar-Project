using Beamable.Server;

namespace Beamable.Server
{
	/// <summary>
	/// This class represents the existence of the StellarFederationStorage database.
	/// Use it for type safe access to the database.
	/// <code>
	/// var db = await Storage.GetDatabase&lt;StellarFederationStorage&gt;();
	/// </code>
	/// </summary>
	[StorageObject("StellarFederationStorage")]
	public class StellarFederationStorage : MongoStorageObject
	{
		
	}
}
