namespace SmashCache
{
	/// <summary>
	/// The available replacement algorithms for the cache.
	/// </summary>
	public enum ReplacementAlgorithm
	{
		LRU,
		MRU,
		ClosestToExpiration,
		Custom
	}
}
 