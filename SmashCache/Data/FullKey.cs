namespace SmashCache.Data
{
	/// <summary>
	/// Object holding both the TagId and the SetId.
	/// </summary>
	public class FullKey
	{
		/// <summary>
		/// Used to look up the data in the DataBlocks within the set.
		/// The key to the Block dictionary
		/// </summary>
		public long TagId { get; set; }

		/// <summary>
		/// Used to look up the set.
		/// The key to the Cache dictionary
		/// </summary>
		public long SetId { get; set; }
	}
}
