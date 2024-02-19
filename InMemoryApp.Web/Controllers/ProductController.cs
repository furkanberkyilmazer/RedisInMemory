using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //1.yol
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToShortTimeString());
            //}

            //2.yol
            //if (!_memoryCache.TryGetValue("zaman",out string zamanCache)) //alabilirse TryGetValue methodu hem geriye true döner hemde zamanCache içine veriyi atar.
            //{
            //    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
            //    cacheOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(30); //30 saniyelik ömür


            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToShortTimeString(), cacheOptions);
            //}


            //her koşulda oluştursun diye
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();

            cacheOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(30); //30 saniyelik ömür
            cacheOptions.SlidingExpiration = TimeSpan.FromSeconds(10); //bu 10 saniye ömrü olur ama 10 saniye içinde birdaha çağırılırsa ömrü 10 saniye daha uzar.  sürekli eski data görmemek için absolute ile birlikte kullanılmalı.
            cacheOptions.Priority = CacheItemPriority.Normal; //memory dolduğunda önce priority si low olanları siler high dersen en sona doğru seni siler. NeverRemove kullanırsan hiç silmez ama bunu kullanırsan ve cache dolarsa exception fırlatır.

            //bir datanın hangi sebepten dolayı memory den düştünü görücez
            cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key} -> {value} =>sebep:{reason}");

            });


            _memoryCache.Set<string>("zaman", DateTime.Now.ToShortTimeString(), cacheOptions);



            Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };
            _memoryCache.Set<Product>("product:1", p);

            return View();
        }
        public IActionResult Show()
        {



            //     //_memoryCache.Remove("zaman");

            //     _memoryCache.GetOrCreate<string>("zaman", entry => { return DateTime.Now.ToString(); }); //almaya çalışır alamazsa entrydeki işi yapar. ve oluşturur zamanın içerisine

            //ViewBag.zaman= _memoryCache.Get<string>("zaman");

            _memoryCache.TryGetValue("zaman", out string zamanCache);
            _memoryCache.TryGetValue("callback", out string callbackCache);
            ViewBag.zaman = zamanCache;
            ViewBag.callback = callbackCache;

            ViewBag.product = _memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
