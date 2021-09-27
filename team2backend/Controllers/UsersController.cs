using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using team2backend.Authentication;
using team2backend.Authentication.Models;
using team2backend.Data;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
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

        [HttpPost]
        public async Task<IActionResult> AddNewUserWithRoles([FromBody] AddNewUser user)
        {
            if (await userManager.FindByEmailAsync(user.Email) != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseAuth { Status = "Error", Message = "User already exists!" });
            }

            ApplicationUser newUser = new()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.Email,
            };

            var roles = roleManager.Roles;

            var result = await userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseAuth { Status = "Error", Message = "User creation failed! Please check user details and try again ." });

            await userManager.AddToRoleAsync(newUser, UserRoles.User);

            foreach (var role in user.Roles)
            {
                if (!await userManager.IsInRoleAsync(newUser, role))
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }

            return Ok(new ResponseAuth { Status = "Success", Message = "User created successfully!" });

        }


        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AddRolesForUser(string id, [FromBody] string[] roles)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, userRoles.ToArray());
                foreach (var role in roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(string id, [FromBody] string role)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.RemoveFromRoleAsync(user, role);
                }

                return Ok();
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
