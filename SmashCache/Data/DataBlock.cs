using System;

namespace SmashCache.Data
{
	/// <summary>
	/// The cached data holder in the cache.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataBlock<T>
	{
		/// <summary>
		/// The actual cached object.
		/// </summary>
		public T CachedData { get; set; }

		/// <summary>
		/// The expiration in UTC.
		/// </summary>
		public DateTime ExpiresAt { get; set; }
		
		/// <summary>
		/// When this cache item was created
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Whether or not the data is expired.
		/// </summary>
		public bool IsExpired => ExpiresAt != null && ExpiresAt < DateTime.UtcNow;
	}

}
 