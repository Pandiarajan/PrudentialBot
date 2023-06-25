using Microsoft.AspNetCore.Mvc;
using NewsAgencies.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NewsAgencies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsAgenciesController : Controller
    {
        string rootPath = AppDomain.CurrentDomain.BaseDirectory; //get the root path
        [HttpGet("GetNewAgenciesData")]
        public JsonResult GetNewAgenciesData()
        {
            Random random = new Random();
            var fullPath = Path.Combine(rootPath, "Content/NewsAgenciesData.json"); //combine the root path with that of our json file inside mydata directory
            var jsonData = System.IO.File.ReadAllText(fullPath); //read all the content inside the file
            var newsNum = random.Next(1, 5); //(inclusive, exclusive);
            var result = JsonConvert.DeserializeObject<List<NewsModel>>(jsonData)?.Where(x => x.Id == newsNum);
            return Json(result);
        }
        [HttpGet("GetPortfolioData")]
        public JsonResult GetPortfolioData()
        {
            var fullPath = Path.Combine(rootPath, "Content/PortfolioData.json"); //combine the root path with that of our json file inside mydata directory
            var jsonData = System.IO.File.ReadAllText(fullPath); //read all the content inside the file
            var result = JsonConvert.DeserializeObject<List<PortfolioModel>>(jsonData);
            return Json(result);
        }
    }
}
