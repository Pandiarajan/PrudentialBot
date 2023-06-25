using Microsoft.AspNetCore.Mvc;
using NewsAgenciesAPI.Model;
using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace NewsAgenciesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewAgenciesController : Controller
    {
        [HttpGet(Name = "GetNewAgenciesData")]
        public JsonResult GetNewAgenciesData()
        {
            Random random= new Random();
            var rootPath = AppDomain.CurrentDomain.BaseDirectory; //get the root path
            var fullPath = Path.Combine(rootPath, "Content/NewsAgenciesData.json"); //combine the root path with that of our json file inside mydata directory
            var jsonData = System.IO.File.ReadAllText(fullPath); //read all the content inside the file
            var newsNum = random.Next(1, 6); //(inclusive, exclusive);
            var result = JsonConvert.DeserializeObject<List<NewsModel>>(jsonData)?.Where(x => x.Id == newsNum);
            return Json(result);
        }
    }
}
