using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions=new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            _distributedCache.SetString("name", "Berk", cacheEntryOptions);
            await _distributedCache.SetStringAsync("surname", "Yılmazer", cacheEntryOptions);

            Product product = new Product { Id = 1, Name = "Kalem", Price = 100 };
            Product product2 = new Product { Id = 2, Name = "Defter", Price = 200 };

            string jsonProduct = JsonConvert.SerializeObject(product);
            string jsonProduct2 = JsonConvert.SerializeObject(product2);


            Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonProduct2);

            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheEntryOptions);
            await _distributedCache.SetAsync("product:2", byteproduct, cacheEntryOptions);

            return View();
        }
        public IActionResult Show()
        {
            string name = _distributedCache.GetString("name");
            string surname = _distributedCache.GetString("surname");
            string productJson = _distributedCache.GetString("product:1");
            Product product = JsonConvert.DeserializeObject<Product>(productJson);

            Byte[] byteproduct = _distributedCache.Get("product:2");
            string jsonProduct=Encoding.UTF8.GetString(byteproduct);
            Product product2 = JsonConvert.DeserializeObject<Product>(jsonProduct);

            ViewBag.name = name;
            ViewBag.surname = surname;
            ViewBag.product = product2;

            return View(product);
        }
        public IActionResult Remove()
        {
           _distributedCache.Remove("name");
            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/car.jpg");

            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("image", imageByte);

            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] resimByte = _distributedCache.Get("image");

            return File(resimByte,"image/jpg");
        }
    }
}
