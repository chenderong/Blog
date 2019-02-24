using Microsoft.Extensions.Caching.Memory;
using System;

namespace XNetCoreCommon
{
    /// <summary>
    /// 缓存助手
    /// </summary>
    public static class MemoryCacheTool
    {
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());


        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCacheValue(string key)
        {
            object val = null;
            if (cache.TryGetValue(key, out val))
                return val;
            else
                return default(object);

        }

        /// <summary>
        /// 添加缓存内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetChacheValue(string key, object value, TimeSpan timeSpan)
        {
            double s = timeSpan.TotalMinutes;
            cache.Set(key, value, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(timeSpan.TotalMinutes)
            });
        }

    }
}
