using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
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
        [EnableCors("CorsApi")]
        [HttpGet]
        [Route("{searchFor}/{numPage}")]
        public IEnumerable<UdemyCourse> Get(string searchFor, int numPage)
        {

            var client = new RestClient($"https://www.udemy.com/api-2.0/courses/?page={numPage}&search={searchFor}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var _apiToken = "Basic Q2thSXFVTURITzREcDk2WGMyejFMd2c5QmN3UzNldFJ2dEhIdUdVRTowaVMyYm9DR05xVm9UYXAwNDZUMXI5VXpKc1ZNWHh4dTRXT3dUUURoV3BhR3JuWkNScndGU2xMN1lyYWVnYXJCTE01UWN3cTVibTl0QW5WUlEyWWg2ME9FeHNWWlJkWG5WcndEdWIyNnlMZE8wSWY0aWVaOXNCV0RtYWpuN1FxNA==";
            request.AddHeader("Authorization", _apiToken);
            IRestResponse response = client.Execute(request);

            return ConvertResponseToUdemyCourse(response.Content);

        }


        [NonAction]
        public IEnumerable<UdemyCourse> ConvertResponseToUdemyCourse(string content)
        {
            var json = JObject.Parse(content);

            return Enumerable.Range(1, 12).Select(index =>

            {
                var results = json["results"][index - 1];
                var instructors = results["visible_instructors"];
                var instructorsLength = instructors.Count();
                var jsonId = results.Value<long>("id");
                var jsonTitle = results.Value<string>("title");
                var jsonUrl = results.Value<string>("url");
                var jsonPrice = results.Value<string>("price");
                var jsonCourseImage = results.Value<string>("image_480x270");
                var jsonHeadline = results.Value<string>("headline");

                return new UdemyCourse
                {
                    Title = jsonTitle,
                    Id = jsonId,
                    Url = jsonUrl,
                    Price = jsonPrice,
                    CourseImage = jsonCourseImage,
                    Headline = jsonHeadline,
                    Instructors = ConvertResponseToInstructors(instructors, instructorsLength)
                };
            }).ToArray();
        }


        [NonAction]
        public IEnumerable<Instructor> ConvertResponseToInstructors(JToken instructors, int instructorsLength)
        {
            return Enumerable.Range(1, instructorsLength).Select(index =>
                    {
                        var jsonInstructorTitle = instructors[index - 1].Value<string>("job_title");
                        var jsonInstructorName = instructors[index - 1].Value<string>("display_name");
                        var jsonInstructorPhoto = instructors[index - 1].Value<string>("image_100x100");
                        return new Instructor
                        {
                            Name = jsonInstructorName,
                            Title = jsonInstructorTitle,
                            Photo = jsonInstructorPhoto
                        };
                    }).ToArray();
        }
    }
}
