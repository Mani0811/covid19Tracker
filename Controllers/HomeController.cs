using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreTodo.Models;
using RestSharp;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCoreTodo.Controllers
{
    public class HomeController : Controller
    {
        private RestClient client = new RestClient("https://api.covid19india.org/");

        private readonly IMemoryCache memoryCache;

        public HomeController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {

            ChartsModel chartsModel = new ChartsModel();
            bool isExist = memoryCache.TryGetValue("Data", out chartsModel);
            if (!isExist)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.covid19india.org/data.json");
                request.Method = "GET";
                string nationDataModelString = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    nationDataModelString = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                var nationDataModel = JsonConvert.DeserializeObject<NationDataModel>(nationDataModelString);
                chartsModel = ChartsModelMapper.Map(nationDataModel);
                var stringModel = JsonConvert.SerializeObject(chartsModel);
                memoryCache.Set("Data", chartsModel, cacheEntryOptions);
            }
            
            return View(chartsModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
