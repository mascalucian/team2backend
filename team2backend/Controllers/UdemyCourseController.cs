using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace team2backend.Controllers
{
    /// <summary>
    ///   <Udemy Controller. />
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UdemyCourseController : ControllerBase
    {
        private const int NUMBER_OF_COURSES_PER_PAGE = 12;

        /// <summary>Gets the specified search for.</summary>
        /// <param name="searchFor">The search for.</param>
        /// <param name="numPage">The number page.</param>
        /// <returns>
        ///   <SearchResult  />
        /// </returns>
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

        /// <summary>Converts the response to udemy course.</summary>
        /// <param name="json">The json.</param>
        /// <param name="numOfCoursesPerThisPage">The number of courses per this page.</param>
        /// <returns>
        ///   <br />
        /// </returns>
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
                    Instructors = ConvertResponseToInstructors(instructors, instructorsLength),
                };
            }).ToArray();
        }

        /// <summary>Converts the response to instructors.</summary>
        /// <param name="instructors">The instructors.</param>
        /// <param name="instructorsLength">Length of the instructors.</param>
        /// <returns>
        ///   <br />
        /// </returns>
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
                    Photo = jsonInstructorPhoto,
                };
            }).ToArray();
        }

        /// <summary>Gets the search results.</summary>
        /// <param name="searchFor">The search for.</param>
        /// <param name="numPage">The number page.</param>
        /// <returns>
        ///   < SearchResult />
        /// </returns>
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

        /// <summary>Gets the last index of the page.</summary>
        /// <param name="numOfCourses">The number of courses.</param>
        /// <returns>LastPage.</returns>
        private static int GetLastPageIndex(long numOfCourses)
        {
            if (numOfCourses % NUMBER_OF_COURSES_PER_PAGE == 0)
            {
                return (int)numOfCourses / NUMBER_OF_COURSES_PER_PAGE;
            }
            else
            {
                return (int)numOfCourses / NUMBER_OF_COURSES_PER_PAGE + 1;
            }
        }

        /// <summary>Gets the number of courses on this page.</summary>
        /// <param name="numPage">The number page.</param>
        /// <param name="numberOfCoursesPerSearch">The number of courses per search.</param>
        /// <param name="lastPage">The last page.</param>
        /// <returns>Number of courses.</returns>
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
            else
            {
                return NUMBER_OF_COURSES_PER_PAGE;
            }
        }

        /// <summary>Gets the response or response error.</summary>
        /// <param name="json">The json.</param>
        /// <param name="numberOfCoursesPerSearch">The number of courses per search.</param>
        /// <param name="numOfCoursesOnThisPage">The number of courses on this page.</param>
        /// <returns>
        ///   <br />
        /// </returns>
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
            else
            {
                return GetResponse(json, numOfCoursesOnThisPage, numberOfCoursesPerSearch);
            }
        }

        /// <summary>Gets the response error.</summary>
        /// <param name="wasOverFullFiled">if set to <c>true</c> [was over full filed].</param>
        /// <param name="noSearchFound">if set to <c>true</c> [no search found].</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private Response GetResponseError(bool wasOverFullFiled, bool noSearchFound)
        {
            return new Response
            {
                WasOverFullFiled = wasOverFullFiled,
                NoSearchFound = noSearchFound,
                NumberOfCoursesFound = 0,
                Courses = Enumerable.Empty<UdemyCourse>(),
            };
        }

        /// <summary>Gets the response.</summary>
        /// <param name="json">The json.</param>
        /// <param name="numOfCoursesPerThisPage">The number of courses per this page.</param>
        /// <param name="numOfCourses">The number of courses.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private Response GetResponse(JObject json, int numOfCoursesPerThisPage, long numOfCourses)
        {
            return new Response
            {
                WasOverFullFiled = false,
                NoSearchFound = false,
                NumberOfCoursesFound = numOfCourses,
                Courses = ConvertResponseToUdemyCourse(json, numOfCoursesPerThisPage),
            };
        }
    }
}
