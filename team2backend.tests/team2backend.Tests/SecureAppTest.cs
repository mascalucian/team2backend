using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace team2backend.Tests
{
    public class SecureAppTest
    {

        [Fact]
        public void CheckConversionToEFConnectionString()
        {
            //Assume

            string databaseUrl = "postgres://mttktcufndbrir:cad4df782d5e7c83ed57cd3b00fc43eeb2c95ca5090548a274eb635fb89e1113@ec2-54-74-77-126.eu-west-1.compute.amazonaws.com:5432/d1gqglci5jubv0";

            //Act

            string convertedConnectionString = Startup.ConvertConnectionString(databaseUrl);

            //Assert

            Assert.Equal("Database=d1gqglci5jubv0;Host=ec2-54-74-77-126.eu-west-1.compute.amazonaws.com;Port=5432;User Id=mttktcufndbrir;Password=cad4df782d5e7c83ed57cd3b00fc43eeb2c95ca5090548a274eb635fb89e1113;SSL Mode=Require;Trust Server Certificate=true", convertedConnectionString);

        }
    }
}
