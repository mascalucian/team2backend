using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
        public Response Get(string searchFor, int numPage)
        {

            var client = new RestClient($"https://www.udemy.com/api-2.0/courses/?page={numPage}&search={HttpUtility.UrlEncode(searchFor)}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var _apiToken = "Basic Q2thSXFVTURITzREcDk2WGMyejFMd2c5QmN3UzNldFJ2dEhIdUdVRTowaVMyYm9DR05xVm9UYXAwNDZUMXI5VXpKc1ZNWHh4dTRXT3dUUURoV3BhR3JuWkNScndGU2xMN1lyYWVnYXJCTE01UWN3cTVibTl0QW5WUlEyWWg2ME9FeHNWWlJkWG5WcndEdWIyNnlMZE8wSWY0aWVaOXNCV0RtYWpuN1FxNA==";
            request.AddHeader("Authorization", _apiToken);
            IRestResponse response = client.Execute(request);
            var json = JObject.Parse(response.Content);
            var numOfCourses = json.Value<long>("count");
            var lastPage = 0;
            var numOfCoursesPerThisPage = 12;
            //Set a value for lastPage
            if (numOfCourses % 12 == 0)
            {
                lastPage = (int)numOfCourses / 12;
            }
            else lastPage = (int)numOfCourses / 12 + 1;

            if(numPage == lastPage)
            {
                numOfCoursesPerThisPage = (int)numOfCourses % 12;
            }

            if (json.Value<string>("detail") == "Invalid page." || json.Value<string>("detail") == "Invalid page size")
            {
                return GetResponseError(true, false, numOfCourses);
            }
            else if(json.Value<long>("count") == 0)
            {
                return GetResponseError(false, true, numOfCourses);
            }
            else return GetResponse(json, numOfCoursesPerThisPage, numOfCourses);

        }

        [NonAction]
        public Response GetResponseError(bool wasOverFullFiled, bool noSearchFound, long numOfCourses)
        {
            return new Response
            {
                WasOverFullFiled = wasOverFullFiled,
                NoSearchFound = noSearchFound,
                NumberOfCoursesFound = numOfCourses,
                Courses = Enumerable.Empty<UdemyCourse>()
            };
        }


    [NonAction]
    public Response GetResponse(JObject json, int numOfCoursesPerThisPage, long numOfCourses)
    {
        return new Response
        {
            WasOverFullFiled = false,
            NoSearchFound = false,
            NumberOfCoursesFound = numOfCourses,
            Courses = ConvertResponseToUdemyCourse(json, numOfCoursesPerThisPage)
        };
    }

    [NonAction]
        public IEnumerable<UdemyCourse> ConvertResponseToUdemyCourse(JObject json, int numOfCoursesPerThisPage)
        {
            return Enumerable.Range(1, numOfCoursesPerThisPage).Select(index =>

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
                        var currentInstructor = instructors[index - 1];
                        var jsonInstructorTitle = currentInstructor.Value<string>("job_title");
                        var jsonInstructorName = currentInstructor.Value<string>("display_name");
                        var jsonInstructorPhoto = currentInstructor.Value<string>("image_100x100");
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
