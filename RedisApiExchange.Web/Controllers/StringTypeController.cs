using Microsoft.AspNetCore.Mvc;
using RedisApiExchange.Web.Services;
using StackExchange.Redis;

namespace RedisApiExchange.Web.Controllers
{
    public class StringTypeController : Controller
    {

        private readonly RedisService _redisService;

        private readonly IDatabase db;

        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetRedisDb(0);
        }

        public IActionResult Index()
        {

            db.StringSet("name", "Berk Yılmazer");
            db.StringSet("ziyaretci", 100);
            return View();
        }
        public IActionResult Show()
        {
            var name = db.StringGet("name");
            var ziyaretci = db.StringGet("ziyaretci");


            //Byte[] resimByte = default(byte[]);
            //db.StringSet("resim", resimByte);

            //var count=db.StringDecrement("ziyaretci", 10); //10 ar azalt
            //var value = db.StringGetRange("ziyaretci",0 ,3); //0 dan başla 3 karakter getir
            //var length= db.StringLength("ziyaretci");

            db.StringIncrement("ziyaretci", 1); //1 arttır

            if (name.HasValue)           
              ViewBag.name = name;
            
             if (true)
                ViewBag.ziyaretci = ziyaretci;


            return View();
        }
    }
}
