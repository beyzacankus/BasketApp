using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.IO;
using System.Diagnostics;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Services;
using Newtonsoft.Json;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IDistributedCache _distributedCache;

        private readonly string ProductRedisKey = "HomeController_Products";
        private readonly DistributedCacheEntryOptions RedisOptions = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(1))
                        .SetAbsoluteExpiration(DateTime.Now.AddMonths(1));
            

        public HomeController(ILogger<HomeController> logger, IProductService productService, IDistributedCache distributedCache)
        {
            _logger = logger;
            _productService = productService;
            _distributedCache = distributedCache;
        }

        public IActionResult Index()
        {
            List<ProductModel> products = _productService.Get();
            List<string> selectedProductIds = new List<string>();

            var productsFromCache = _distributedCache.Get(ProductRedisKey);
            string json;

            if (productsFromCache != null)
            {
                json = Encoding.UTF8.GetString(productsFromCache);
                var productIdList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);

                if (productIdList != null)
                    selectedProductIds.AddRange(productIdList);
            }

            ViewBag.SelectedProductIds = selectedProductIds;

            return View(products);
        }

        public IActionResult Basket()
        {
            List<string> selectedProductIds = new List<string>();

            var productsFromCache = _distributedCache.Get(ProductRedisKey);
            string json;

            if (productsFromCache != null)
            {
                json = Encoding.UTF8.GetString(productsFromCache);
                var productIdList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);

                if (productIdList != null)
                    selectedProductIds.AddRange(productIdList);
            }


            List<ProductModel> products = _productService.GetByIds(selectedProductIds);
            
            return View(products);
        }

        [HttpPost]
        public JsonResult AddOrRemoveBasket(string id)
        {
            var productsFromCache = _distributedCache.Get(ProductRedisKey);
            List<string> productIdList = new List<string>();
            string json;

            if (productsFromCache != null)
            {
                json = Encoding.UTF8.GetString(productsFromCache);
                productIdList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }            

            bool isAdded = true;

            if (productIdList.Contains(id))
            {
                productIdList.Remove(id);
                isAdded = false;
            }
            else
            {
                productIdList.Add(id);
            }

            json = Newtonsoft.Json.JsonConvert.SerializeObject(productIdList);
            productsFromCache = Encoding.UTF8.GetBytes(json);

            _distributedCache.Set(ProductRedisKey, productsFromCache, RedisOptions);

            return Json(isAdded);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}