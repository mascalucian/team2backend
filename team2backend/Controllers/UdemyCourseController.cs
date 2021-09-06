using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace team2backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UdemyCourseController : ControllerBase
    {
        private const int NUMBER_OF_COURSES_PER_PAGE = 12;
        [EnableCors("CorsApi")]
        [HttpGet]
        [Route("{searchFor}/{numPage}")]
        public Response Get(string searchFor, int numPage)
        {
            string content = GetSearchResults(searchFor, numPage);
            var json = JObject.Parse(content);
            var numberOfCoursesPerSearch = json.Value<long>("count");
            int lastPage = GetLastPageIndex(numberOfCoursesPerSearch);
            var numOfCoursesOnThisPage = GetNumberOfCoursesOnThisPage(numPage, numberOfCoursesPerSearch, lastPage);
            return GetResponseOrResponseError(json, numberOfCoursesPerSearch, numOfCoursesOnThisPage);
        }

        private static string GetSearchResults(string searchFor, int numPage)
        {
            var client = new RestClient($"https://www.udemy.com/api-2.0/courses/?page={numPage}&search={HttpUtility.UrlEncode(searchFor)}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var _apiToken = "Basic Q2thSXFVTURITzREcDk2WGMyejFMd2c5QmN3UzNldFJ2dEhIdUdVRTowaVMyYm9DR05xVm9UYXAwNDZUMXI5VXpKc1ZNWHh4dTRXT3dUUURoV3BhR3JuWkNScndGU2xMN1lyYWVnYXJCTE01UWN3cTVibTl0QW5WUlEyWWg2ME9FeHNWWlJkWG5WcndEdWIyNnlMZE8wSWY0aWVaOXNCV0RtYWpuN1FxNA==";
            request.AddHeader("Authorization", _apiToken);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return content;
        }

        private static int GetLastPageIndex(long numOfCourses)
        {
            if (numOfCourses % NUMBER_OF_COURSES_PER_PAGE == 0)
            {
                return (int)numOfCourses / NUMBER_OF_COURSES_PER_PAGE;
            }
            else return (int)numOfCourses / NUMBER_OF_COURSES_PER_PAGE + 1;
        }

        private static int GetNumberOfCoursesOnThisPage(int numPage, long numberOfCoursesPerSearch, int lastPage)
        {
            if (numPage == lastPage)
            {
                if ((int)numberOfCoursesPerSearch % NUMBER_OF_COURSES_PER_PAGE == 0)
                {
                    return NUMBER_OF_COURSES_PER_PAGE;
                }
                else
                {
                    return (int)numberOfCoursesPerSearch % NUMBER_OF_COURSES_PER_PAGE;
                }
            }
            else return NUMBER_OF_COURSES_PER_PAGE;
        }

        private Response GetResponseOrResponseError(JObject json, long numberOfCoursesPerSearch, int numOfCoursesOnThisPage)
        {
            var PageNotFound = json.Value<string>("detail");

            if (PageNotFound == "Invalid page." || PageNotFound == "Invalid page size")
            {
                return GetResponseError(true, false);
            }
            else if (numberOfCoursesPerSearch == 0)
            {
                return GetResponseError(false, true);
            }
            else return GetResponse(json, numOfCoursesOnThisPage, numberOfCoursesPerSearch);
        }

        private Response GetResponseError(bool wasOverFullFiled, bool noSearchFound)
        {
            return new Response
            {
                WasOverFullFiled = wasOverFullFiled,
                NoSearchFound = noSearchFound,
                NumberOfCoursesFound = 0,
                Courses = Enumerable.Empty<UdemyCourse>()
            };
        }

        private Response GetResponse(JObject json, int numOfCoursesPerThisPage, long numOfCourses)
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

        private IEnumerable<Instructor> ConvertResponseToInstructors(JToken instructors, int instructorsLength)
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
