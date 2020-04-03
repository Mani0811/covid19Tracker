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

namespace AspNetCoreTodo.Controllers
{
    public class HomeController : Controller
    {
        private RestClient client = new RestClient("https://api.covid19india.org/");

        public IActionResult Index()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.covid19india.org/data.json");
            request.Method = "GET";
            String test = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                test = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }

            NationDataModel nationDataModel = JsonConvert.DeserializeObject<NationDataModel>(test);

            return View(nationDataModel);
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
