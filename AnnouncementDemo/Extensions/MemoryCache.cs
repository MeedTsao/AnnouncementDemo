using System;
using System.Runtime.Caching;

namespace AnnouncementDemo.Extensions
{
	public class AppCache
	{
		#region 屬性
		private ObjectCache Cache
		{
			get
			{
				return MemoryCache.Default;
			}
		}

		// 因為與其他應用程式共用此記憶體快取，所以建議增加此應用程式的前置名稱
		private string IdNameStart = "Anno_";
		#endregion

		#region 建構子
		public AppCache()
		{

		}
		#endregion


		#region 方法
		/// <summary>
		/// 取得快取
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(string key)
		{
			return Cache[IdNameStart + key];
		}

		/// <summary>
		/// 移除快取
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			Cache.Remove(IdNameStart + key);
		}

		/// <summary>
		/// 是否存在快取
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsSet(string key)
		{
			return (Cache[IdNameStart + key] != null);
		}

		/// <summary>
		/// 設定快取
		/// </summary>
		/// <param name="key">KEY</param>
		/// <param name="data">資料</param>
		public void Set(string key, object data)
		{
			this.Set(key, data, "Sliding", 60);
		}

		/// <summary>
		/// 設定快取
		/// </summary>
		/// <param name="key">KEY</param>
		/// <param name="data">資料</param>
		/// <param name="Expiration">保留別</param>
		/// <param name="cacheTime">保存時間(分鐘)</param>
		public void Set(string key, object data, string Expiration, int cacheTime)
		{
			CacheItemPolicy policy = new CacheItemPolicy();
			if (Expiration == "Absolute")
			{
				policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
			}
			else if (Expiration == "Sliding")
			{
				policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTime);
			}
			Cache.Add(new CacheItem(IdNameStart + key, data), policy);
		}
		#endregion
	}
}
