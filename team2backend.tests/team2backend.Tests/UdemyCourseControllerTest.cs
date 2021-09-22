using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using team2backend.Controllers;
using team2backend.Services;
using Xunit;

namespace team2backend.Tests
{
    public class UdemyCourseControllerTest
    {
        [Fact]
        public void SetApiKeyTest()
        {
            var output = UdemyCourseService.GetApiKey();
            Assert.Equal("Q2thSXFVTURITzREcDk2WGMyejFMd2c5QmN3UzNldFJ2dEhIdUdVRTowaVMyYm9DR05xVm9UYXAwNDZUMXI5VXpKc1ZNWHh4dTRXT3dUUURoV3BhR3JuWkNScndGU2xMN1lyYWVnYXJCTE01UWN3cTVibTl0QW5WUlEyWWg2ME9FeHNWWlJkWG5WcndEdWIyNnlMZE8wSWY0aWVaOXNCV0RtYWpuN1FxNA==",
                output);
        }

        [Fact]
        public void HasResultsTest()
        {
            var illegalQuery = "Java";
            var service = new UdemyCourseService();
            var output = service.HasResults(illegalQuery);
            Assert.Equal(true, output);
        }
    }
}
