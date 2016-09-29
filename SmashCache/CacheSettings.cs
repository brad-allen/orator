namespace SmashCache
{
	/// <summary>
	/// Data class to pass into Cache creation in order to override defaults.
	/// Can only be updated if the cache is empty.
	/// CacheDefaultExpirationMinutes can be overridden per cache item set.
	/// </summary>
	public class CacheSettings
	{
		private const int DefaultNumberOfSets = 32;
		private const int DefaultBlocksPerSet = 128;
		private const int DefaultCacheExpirationMinutes = 60;

		/// <summary>
		/// The number of cache sets for this instance of the cache.
		/// </summary>
		public int NumberOfSets { get; set; } = DefaultNumberOfSets;

		/// <summary>
		/// The number of blocks per set for this instance of the cache.
		/// </summary>
		public int BlocksPerSet { get; set; } = DefaultBlocksPerSet;

		/// <summary>
		/// The default expiration for items in the cache.
		/// The expiration may also be overridden for a specific item when the item is set in the cache.
		/// </summary>
		public int CacheDefaultExpirationMinutes { get; set; } = DefaultCacheExpirationMinutes;
		
		/// <summary>
		/// The default replacement/eviction algorithm to be used for this instance of the cache.
		/// Set to Custom and override CustomReplacementAlgorithm to create own replacement algorithm.
		/// </summary>
		public ReplacementAlgorithm ReplacementAlgorithm { get; set; } = ReplacementAlgorithm.LRU;

	}
}
 