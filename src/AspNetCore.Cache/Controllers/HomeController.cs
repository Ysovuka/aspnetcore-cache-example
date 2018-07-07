using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Cache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCore.Cache.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;

        public HomeController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            DateTime cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue("DateTime", out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = DateTime.Now;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                // Save data in cache.
                _cache.Set("DateTime", cacheEntry, cacheEntryOptions);
            }

            return View("Cache", cacheEntry);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CacheTryGetValueSet()
        {
            DateTime cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue("DateTime", out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = DateTime.Now;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                // Save data in cache.
                _cache.Set("DateTime", cacheEntry, cacheEntryOptions);
            }

            return View("Cache", cacheEntry);
        }

        public IActionResult CacheGetOrCreate()
        {
            var cacheEntry = _cache.GetOrCreate("DateTime", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(3);
                return DateTime.Now;
            });

            return View("Cache", cacheEntry);
        }

        public async Task<IActionResult> CacheGetOrCreateAsync()
        {
            var cacheEntry = await
                _cache.GetOrCreateAsync("DateTime", entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromSeconds(3);
                    return Task.FromResult(DateTime.Now);
                });

            return View("Cache", cacheEntry);
        }

        public IActionResult CacheGet()
        {
            var cacheEntry = _cache.Get<DateTime?>("DateTime");
            return View("Cache", cacheEntry);
        }

        public IActionResult CacheRemove()
        {
            _cache.Remove("DateTime");
            var cacheEntry = _cache.Get<DateTime>("DateTime");

            return View("Cache", cacheEntry);
        }
    }
}
