using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace team2backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UdemyCourseController : ControllerBase
    {

        [HttpGet]
        [Route("{searchFor}/{numPages}")]
        public IEnumerable<UdemyCourse> Get(string searchFor, int numPages)
        {

            var client = new RestClient($"https://api.openweathermap.org/data/2.5/weather?q={City}&appid=5705b52fdd12e0753d98f978798de52a");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var course = new UdemyCourse();

            return course;
        }


        [NonAction]
        public IEnumerable<UdemyCourse> ConvertResponseToUdemyCourse(string content, int numPages = 12)
        {
            var json = JObject.Parse(content);


            var rng = new Random();
            return Enumerable.Range(1, numPages).Select(index =>

            {
                var jsonDailyForecast = json["daily"][index];
                var unixDateTime = jsonDailyForecast.Value<long>("dt");
                var weatherSummary = jsonDailyForecast["weather"][0].Value<string>("main");

                return new WeatherForecast
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds(unixDateTime).Date,
                    TemperatureC = ExtractCelsiusTemperatureFromDailyWeather(jsonDailyForecast),
                    Summary = weatherSummary
                };
            })
            .ToArray();
        }
    }
}
