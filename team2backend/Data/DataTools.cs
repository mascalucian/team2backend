using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Authentication;
using team2backend.Models;

namespace team2backend.Data
{
    public static class DataTools
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var applicationDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

             
                if (applicationDbContext.Skills.Any())
                {
                    Console.WriteLine("the skills are there");
                }
                else
                {
                    var skill1 = new Skill
                    {
                        Id = 1,
                        Name = "Java",
                        ParentId = 3,
                    };
                    applicationDbContext.Add(skill1);
                    var skill2 = new Skill
                    {
                        Id = 2,
                        Name = "c#",
                        ParentId = 4,
                    };
                    applicationDbContext.Add(skill2);
                    applicationDbContext.SaveChanges();
                }

                if (applicationDbContext.Recomandations.Any())
                {
                    Console.WriteLine("the Recomandations are there");
                }
                else
                {
                    var Recomandation1 = new Recomandation
                    {
                        CourseId = 3,
                        CourseTitle = "Java",
                        Rating = 3,
                        UserName = "OvidiuA",
                        Feedback = "Good Enough",
                        SkillId = 1,
                        SkillName = "Java",
                        UserId = "Da2aSG23A",
                    };
                    applicationDbContext.Add(Recomandation1);
                    var Recomandation2 = new Recomandation
                    {
                        CourseId = 4,
                        CourseTitle = "c#",
                        Rating = 4,
                        UserName = "OvidiuA",
                        Feedback = "Good",
                        SkillId = 2,
                        SkillName = "c#",
                        UserId = "Da2aSG23A",
                    };
                    applicationDbContext.Add(Recomandation2);
                    applicationDbContext.SaveChanges();
                }
            }
        }
    }
}