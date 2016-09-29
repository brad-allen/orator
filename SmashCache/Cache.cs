using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using SmashCache.Data;

namespace SmashCache
{
	/// <summary>
	/// Generic Set Associative Cache.
	/// All objects must be serializable.
	/// </summary>
	/// <typeparam name="Tkey"></typeparam>
	/// <typeparam name="Tvalue"></typeparam>
	public class Cache<Tkey, Tvalue> : IDisposable
	{
		private Dictionary<long, CacheSet<Tvalue>> _cache = new Dictionary<long, CacheSet<Tvalue>>();

		/// <summary>
		/// The current settings for the cache.
		/// May be updated through UpdateCacheSettings(), only if there is no data in the cache.
		/// </summary>
		public CacheSettings CacheSettings { get; private set; }  = new CacheSettings();
		
		/// <summary>
		/// Set the item in the cache.
		/// Returns whether or not the set was successful.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expirationInMinutes"></param>
		/// <returns>bool - whether the set was successful</returns>
		public bool Set(Tkey key, Tvalue value, int? expirationInMinutes = null)
		{
			try
			{
				var fullKey = FullKeyFromObject(key);
				var cacheSet = _cache.ContainsKey(fullKey.SetId) ? _cache[fullKey.SetId] : null;
				
				if (cacheSet?.Blocks != null) //set exists
				{
					_cache.Remove(fullKey.SetId);
					if (cacheSet.Blocks.ContainsKey(fullKey.TagId))//tag exists, just replace it
					{
						cacheSet.Blocks.Remove(fullKey.TagId);
						cacheSet.LastUsed.Remove(fullKey.TagId);
					}
					else if (cacheSet.Blocks.Count() >= CacheSettings.BlocksPerSet) 
					{
						EvictBlock(cacheSet);
					}
				}
				else
				{
					cacheSet = new CacheSet<Tvalue>();
				}

				var dataBlock = new DataBlock<Tvalue> { CachedData = value, ExpiresAt = DateTime.UtcNow.AddMinutes(expirationInMinutes ?? CacheSettings.CacheDefaultExpirationMinutes), CreatedAt = DateTime.UtcNow };
				cacheSet.Blocks.Add(fullKey.TagId, dataBlock);
				_cache.Add(fullKey.SetId, cacheSet);
				return true;
			}
			catch (System.Exception e)
			{
				//TODO handle exceptions!
				return false;
			}
		}
		
		/// <summary>
		/// Try to find the value.
		/// Returns true if found.
		/// Value is returned in ref variable.
		/// The default value is returned if not found.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>bool - if it was found or not</returns>
		public bool TryFind(Tkey key, ref Tvalue value)
		{
			try
			{
				value = default(Tvalue);
				var fullKey = FullKeyFromObject(key);
				var set = _cache.ContainsKey(fullKey.SetId) ? _cache[fullKey.SetId] : null;

				//Returns the value, but a false if it is expired but still in the cache
				if (((set?.Blocks?.ContainsKey(fullKey.TagId) ?? false) && !set.Blocks[fullKey.TagId].IsExpired))
				{
					set.LastUsed[fullKey.TagId] = DateTime.UtcNow;
					value = set.Blocks[fullKey.TagId].CachedData;
					return true;
				}
				return false;
			}
			catch(System.Exception)
			{
				//Log exception
				value = default(Tvalue);
				return false;
			}
		}

		/// <summary>
		/// Find the value.
		/// Returns a default object if not found.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns>bool - if it was found or not</returns>
		public Tvalue Find(Tkey key)
		{
			try
			{
				var fullKey = FullKeyFromObject(key);
				var set = _cache.ContainsKey(fullKey.SetId) ? _cache[fullKey.SetId] : null;
				
				if (((set?.Blocks?.ContainsKey(fullKey.TagId) ?? false) && !set.Blocks[fullKey.TagId].IsExpired))
				{
					set.LastUsed[fullKey.TagId] = DateTime.UtcNow;
					return set.Blocks[fullKey.TagId].CachedData;
				}
				return default(Tvalue);
			}
			catch (System.Exception)
			{
				//Log exception
				return default(Tvalue);
			}
		}

		/// <summary>
		/// Overload this method to supply your own custom replacement/eviction algorithm.
		/// Use to remove a Block from the current cacheSet so another block of data may be placed into the set.
		/// eg: cacheSet.Blocks.Remove(cacheSet.Blocks.OrderByDescending(m => m.Value.ExpiresAt).FirstOrDefault().Key);
		/// </summary>
		/// <param name="cacheSet"></param>
		public virtual void CustomReplacementAlgorithm(CacheSet<Tvalue> cacheSet)
		{
			LRUReplacementAlgorithm(cacheSet);
		}

		/// <summary>
		/// Update the cache settings
		/// Will only update if there is no data in the cache
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public bool UpdateCacheSettings(CacheSettings settings = null)
		{
			if (settings != null && !_cache.Any())
			{
				CacheSettings = settings;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sweep the item from the cache using the specified key.
		/// Returns whether the item was found and successfully removed.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>bool - if found and successfully removed</returns>
		public bool SweepItem(Tkey key)
		{
			var fullKey = FullKeyFromObject(key);
			var set = _cache.ContainsKey(fullKey.SetId) ? _cache[fullKey.SetId] : null;

			if (set?.Blocks?.ContainsKey(fullKey.TagId) ?? false)
			{
				return set.Blocks.Remove(fullKey.TagId);
			}
			return false;
		}

		public void Dispose()
		{
			_cache.Clear();
		}

		private void EvictBlock(CacheSet<Tvalue> cacheSet)
		{
			var oldestExpired = cacheSet.Blocks.OrderByDescending(m => m.Value.ExpiresAt).Where(i => i.Value.IsExpired == true).FirstOrDefault();

			//try to remove the oldest expired one first
			if (oldestExpired.Value != null)
			{
				cacheSet.Blocks.Remove(oldestExpired.Key);
				cacheSet.LastUsed.Remove(oldestExpired.Key);
			}
			else
			{
				//else evict by algorithm
				switch (CacheSettings.ReplacementAlgorithm)
				{
					case ReplacementAlgorithm.LRU:
						LRUReplacementAlgorithm(cacheSet);
						break;
					case ReplacementAlgorithm.MRU:
						MRUReplacementAlgorithm(cacheSet);
						break;
					case ReplacementAlgorithm.ClosestToExpiration:
						ClosestToExpirationAlgorithm(cacheSet);
						break;
					case ReplacementAlgorithm.Custom:
						CustomReplacementAlgorithm(cacheSet);
						break;
				}
			}
		}

		private void LRUReplacementAlgorithm(CacheSet<Tvalue> cacheSet)
		{
			var leastRecentlyUsed = cacheSet.LastUsed.OrderBy(m => m.Value).FirstOrDefault();
			cacheSet.LastUsed.Remove(leastRecentlyUsed.Key);
			cacheSet.Blocks.Remove(leastRecentlyUsed.Key);
		}
		
		private void MRUReplacementAlgorithm(CacheSet<Tvalue> cacheSet)
		{
			var mostRecentlyUsed = cacheSet.LastUsed.OrderByDescending(m => m.Value).FirstOrDefault();
			cacheSet.LastUsed.Remove(mostRecentlyUsed.Key);
			cacheSet.Blocks.Remove(mostRecentlyUsed.Key);
		}

		private void ClosestToExpirationAlgorithm(CacheSet<Tvalue> cacheSet)
		{
			var oldestAdded = cacheSet.Blocks.OrderBy(m => m.Value.ExpiresAt).FirstOrDefault();
			cacheSet.LastUsed.Remove(oldestAdded.Key);
			cacheSet.Blocks.Remove(oldestAdded.Key);
		}

		private FullKey FullKeyFromObject(object key)
		{
			if (key == null) return null;

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, key);
				return GetFullKey(BitConverter.ToString(memoryStream.ToArray()));
			}
		}
		
		private FullKey GetFullKey(string keyString)
		{
			var intKey = CreateIntegerKey(keyString);
			var set = intKey % CacheSettings.NumberOfSets;
			return new FullKey { SetId = set, TagId = intKey };
		}

		//based off of: https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5
		private long CreateIntegerKey(string key)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes(key);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}

				return BitConverter.ToInt64(Encoding.UTF8.GetBytes(sb.ToString()), 1);
			}
		}
	}
}
 