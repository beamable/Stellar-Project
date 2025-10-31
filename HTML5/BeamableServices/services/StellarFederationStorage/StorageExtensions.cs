using Beamable.Common;
using MongoDB.Driver;

namespace Beamable.Server
{
	public static class StellarFederationStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for StellarFederationStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> StellarFederationStorageDatabase(
			this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<StellarFederationStorage>();

		/// <summary>
		/// Gets a MongoDB collection from StellarFederationStorage by the requested name, and uses the given mapping class.
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <param name="provider">The name of the provider</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> StellarFederationStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<StellarFederationStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from StellarFederationStorage by the requested name, and uses the given mapping class.
		/// </summary>
		/// <param name="provider">The name of the provider</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> StellarFederationStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<StellarFederationStorage, TCollection>();
	}
}