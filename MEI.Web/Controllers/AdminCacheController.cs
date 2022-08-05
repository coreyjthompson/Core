using System.Reflection;

using LazyCache;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MEI.Web.Controllers
{
    [Authorize(Policy = "IsJasonNichols")]
    public class AdminCacheController
        : Controller
    {
        private readonly IAppCache _cache;
        private readonly IMemoryCache _memoryCache;

        public AdminCacheController(IAppCache cache, IMemoryCache memoryCache)
        {
            _cache = cache;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            PropertyInfo prop = _memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            object innerCache = prop.GetValue(_memoryCache);
            MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(innerCache, null);

            return View();
        }
    }
}
