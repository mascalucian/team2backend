using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Authentication;
using team2backend.Data;
using team2backend.Dtos;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userDto = mapper.Map<ApplicationUser, ReadUserDto>(user);
                var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                userDto.Roles = roles;
                return Ok(userDto);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var task = Task.Run(() => userManager.Users.ToList());
            var users = await task;
            var usersDto = mapper.Map<List<ApplicationUser>, List<ReadUserDto>>(users);
            foreach (var user in usersDto)
            {
                var userTask = Task.Run(() => userManager.FindByIdAsync(user.Id));
                var identityUser = await userTask;
                var rolesTask = Task.Run(() => userManager.GetRolesAsync(identityUser));
                user.Roles = await rolesTask;
            }
            return Ok(usersDto);
        }
    }
}
