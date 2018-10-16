using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Ranking.Infrastructure
{
    public interface ICacheManager
    {
        object Get(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Invalidate(string key);
    }

    public class CacheManager : ICacheManager
    {
        public const string RankingCacheKey = "RankingCacheKey";
        public const string TeamListCacheKey = "TeamListCacheKey";
        public const string RankArchCacheKey = "RankArchCacheKey";
        public const string FinishLeagueCacheKey = "FinishLeagueCacheKey";
        private Cache cache { get { return HttpContext.Current.Cache; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return cache[key];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Invalidate(string key)
        {
            cache.Remove(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            return (cache[key] != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Set(string key, object data, int cacheTime)
        {
            var expirationTime = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            cache.Insert(key, data, null, expirationTime, Cache.NoSlidingExpiration);
        }
    }
}