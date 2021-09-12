using Newtonsoft.Json.Linq;
using System;
using System.IO;
using team2backend.Controllers;
using Xunit;

namespace team2backend.Tests
{
    public class UdemyCourseControllerTest
    {
        [Fact]
        public void ConvertResponseToUdemyCourseTest()
        {
            // Assume
            string content = LoadJsonFromResource("DataFromUdemyCoursesApi.json");
            var controller = new UdemyCourseController();
            // Act
            var output = controller.ConvertResponseToUdemyCourse(JObject.Parse(content), 12);

            // Assert
            var FirstCourse = ((UdemyCourse[])output)[0];
            Assert.Equal("C Programming For Beginners - Master the C Language", FirstCourse.Title);
            Assert.Equal("/course/c-programming-for-beginners-/", FirstCourse.Url);
        }

        private string LoadJsonFromResource(string jsonResponseOfApiFile)
        {
            var assembly = this.GetType().Assembly;
            var assemblyName = assembly.GetName().Name;
            var resourceName = $"{assemblyName}.{jsonResponseOfApiFile}";
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            using (var tr = new StreamReader(resourceStream))
            {
                return tr.ReadToEnd();
            }
        }
    }
}
