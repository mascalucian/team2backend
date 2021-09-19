using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using team2backend.Authentication;
using team2backend.Authentication.Models;
using team2backend.Data;
using team2backend.DTOs;
using team2backend.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        //private readonly IMapper mapper;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
            //this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            //var users = context.Users.ToList();
            //Console.WriteLine(users);
            //Console.WriteLine(userManager.Users.ForEachAsync(user => new User()
            //{
            //    Id = user.Id,
            //    Username = user.UserName,
            //    Email = user.Email,
            //    Recommendations = null,
            //}));
            return Ok();
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var currentUser = context.Users.FirstOrDefault(user => user.Id == id);
                User user = new ();
                user.Username = currentUser.UserName;
                user.Email = currentUser.Email;
                var recommendations = await context.Recommendations.Where(recommendation => recommendation.UserId == currentUser.Id)
                .ToListAsync();
                user.Recommendations = recommendations;
                //ReadUserRecommendationsDTO readUserRecommendationsDTO = mapper.Map<ReadUserRecommendationsDTO>(user);
                return Ok(user);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("userId")]
        public async Task<IActionResult> GetRecomandationByUserId(string id)
        {
            try
            {
                var recomandations = await context.Recommendations.Where(recommendationId => recommendationId.UserId == id)
                .Distinct().ToListAsync();
                return Ok(recomandations);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
