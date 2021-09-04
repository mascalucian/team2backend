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

            var client = new RestClient($"https://www.udemy.com/api-2.0/courses/?page={numPages}&search={searchFor}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return ConvertResponseToUdemyCourse(response.Content, numPages);
        }


        [NonAction]
        public IEnumerable<UdemyCourse> ConvertResponseToUdemyCourse(string content, int numPages = 12)
        {
            var json = JObject.Parse(content);


            var rng = new Random();
            return Enumerable.Range(1, numPages).Select(index =>

            {
                var results = json["results"];
                var instructor = results["visible_instructors"];
                var jsonTitle = results.Value<string>("title");
                var jsonUrl = results.Value<string>("url");
                var jsonPrice = results.Value<string>("price");
                var jsonCourseImage = results.Value<string>("image_240x135");
                var jsonHeadline = results.Value<string>("headline");
                var jsonInstructorTitle = instructor.Value<string>("job_title");
                var jsonInstructorName = instructor.Value<string>("display_name");
                var jsonInstructorPhoto = instructor.Value<string>("image_100x100");

                return new UdemyCourse
                {
                    Title = jsonTitle,
                    Url = jsonUrl,
                    Price = jsonPrice,
                    CourseImage = jsonCourseImage,
                    Headline = jsonHeadline,
                    Instructor = new Instructor
                    {
                        Name = jsonInstructorName,
                        Title = jsonInstructorTitle,
                        Photo = jsonInstructorPhoto
                    }
                };
            })
            .ToArray();
        }
    }
}
