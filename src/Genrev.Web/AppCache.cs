using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Genrev.Web
{
    static class AppCache
    {

        const int CACHE_TIMEOUT_MINUTES = 20;

        public static void AddItem(string key, object item) {
            MemoryCache c = MemoryCache.Default;
            c.Set(key, item, DateTimeOffset.Now.AddMinutes(CACHE_TIMEOUT_MINUTES));
        }

        public static object GetItem(string key) {
            return MemoryCache.Default.Get(key);
        }

        public static void InvalidateItem(string key) {
            MemoryCache.Default.Remove(key);
        }

        public static void InvalidateAll() {
            MemoryCache.Default.Trim(100);
        }


    }
}