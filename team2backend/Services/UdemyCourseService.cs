using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace team2backend.Services
{
    public class UdemyCourseService : IUdemyCourseService
    {
        private static readonly string API_KEY;
        private static readonly int NUMBER_OF_COURSES_PER_PAGE = 12;
        private static IConfigurationRoot Configuration { get; set; }

        static UdemyCourseService()
        {
            Init();
            API_KEY = GetApiKey();
        }

        private static void Init()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public static string GetApiKey()
        {
            var env = Environment.GetEnvironmentVariable("API_KEY");
            if (env != null)
            {
                return env;
            }
            return Configuration["Udemy:ApiKey"];
        }

        public Response GetResponse(string search, int page)
        {
            var json = GetResults(search, page);
            var numberOfCoursesPerSearch = json.Value<long>("count");
            return GetResponseOrResponseError(json, numberOfCoursesPerSearch);
        }

        public Boolean HasResults(string search)
        {
            var json = GetResults(search, 1);
            var numberOfCoursesPerSearch = json.Value<long>("count");
            return (numberOfCoursesPerSearch != 0);
        }

        private JObject GetResults(string search, int page)
        {
            var client = new RestClient($"https://www.udemy.com/api-2.0/courses/?page={page}&search={HttpUtility.UrlEncode(search)}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Basic {API_KEY}");
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return JObject.Parse(content);
        }

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

        private Response GetResponse(JObject json, long numOfCourses)
        {
            return new Response
            {
                WasOverFullFiled = false,
                NoSearchFound = false,
                NumberOfCoursesFound = numOfCourses,
                Courses = ConvertResponseToUdemyCourse(json),
            };
        }

        private Response GetResponseOrResponseError(JObject json, long numberOfCoursesPerSearch)
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
                return GetResponse(json, numberOfCoursesPerSearch);
            }
        }

        private IEnumerable<UdemyCourse> ConvertResponseToUdemyCourse(JObject json)
        {
            return Enumerable.Range(1, json["results"].Count()).Select(index =>
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
                    Instructors = ConvertResponseToInstructors(instructors),
                };
            }).ToArray();
        }

        private IEnumerable<Instructor> ConvertResponseToInstructors(JToken instructors)
        {
            return Enumerable.Range(1, instructors.Count()).Select(index =>
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

    }
}
