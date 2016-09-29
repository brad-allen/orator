using System;
using System.Collections.Generic;
using SmashCache.Data;

namespace SmashCache
{
	/// <summary>
	/// Set that contains the Blocks dictionary in which to store DataBlock data.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CacheSet<T> : IDisposable
	{
		/// <summary>
		/// The dictionary of DataBlock data which contains the cached information.
		/// </summary>
		public Dictionary<long, DataBlock<T>> Blocks = new Dictionary<long, DataBlock<T>>();
		
		/// <summary>
		/// Dictionary of the the LastUsed date for the items in the BLocks dictionary
		/// </summary>
		public Dictionary<long, DateTime> LastUsed = new Dictionary<long, DateTime>();

		public void Dispose()
		{
			Blocks.Clear();
		}
	}
}
