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
            //chartsModel = GetDataWithoutCache();
            var chartsModel = GetDataWithCache();
            return View(chartsModel);
        }

        private ChartsModel GetDataWithCache()
        {
            ChartsModel chartsModel;
            bool isExist = memoryCache.TryGetValue("Data", out chartsModel);
            if (!isExist)
            {
                chartsModel = GetDataFromThirdParty();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                memoryCache.Set("Data", chartsModel, cacheEntryOptions);
            }

            return chartsModel;
        }

        private static ChartsModel GetDataFromThirdParty()
        {
            ChartsModel chartsModel;
            string nationUrl = "https://api.covid19india.org/data.json";
            string nationDataModelString = GetData(nationUrl);
            var nationDataModel = JsonConvert.DeserializeObject<NationDataModel>(nationDataModelString);

            var stateUrl = "https://api.covid19india.org/v2/state_district_wise.json";
            string stateDistrictDataModelString = GetData(stateUrl);
            var stateDistrictModel = JsonConvert.DeserializeObject<List<StateDistrictModel>>(stateDistrictDataModelString);
            chartsModel = ChartsModelMapper.Map(nationDataModel, stateDistrictModel);
            return chartsModel;
        }

        private static string GetData(string url)
        {
            string nationDataModelString;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                nationDataModelString = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }

            return nationDataModelString;
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
