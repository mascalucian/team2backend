using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly IHubContext<MessageHub> hubContext;

        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IHubContext<MessageHub> hubContext)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.context = context;
            this.hubContext = hubContext;
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

            ApplicationUser newUser = new ()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.Email,
            };

            var badRoles = new List<string>();
            var goodRoles = new List<string>();
            foreach (var role in user.Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    badRoles.Add(role);
                }
                else
                {
                    goodRoles.Add(role);
                }
            }

            var result = await userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseAuth { Status = "Error", Message = "User creation failed! Please check user details and try again ." });

            var badRolesInString = string.Empty;
            foreach (var badRole in badRoles)
            {
                badRolesInString = badRolesInString + " " + badRole;
            }

            await userManager.AddToRoleAsync(newUser, UserRoles.User);

            if (goodRoles.Count > 0)
            {
                foreach (var role in goodRoles)
                {
                    if (!await userManager.IsInRoleAsync(newUser, role))
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                    }
                }
            }
            else
            {
                await userManager.AddToRoleAsync(newUser, "User");
            }

            var userDto = mapper.Map<ReadUserDto>(user);
            await hubContext.Clients.All.SendAsync("UserAdded", userDto);
            if (badRoles.Count == 0)
            {
                return Ok(new ResponseAuth { Status = "Success", Message = "User created successfully!" });
            }
            else
            {
                return Ok(new ResponseAuth { Status = "Success", Message = $"User created successfully, but roles {badRolesInString} are not valid!" });
            }
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                context.Recomandations.RemoveRange(context.Recomandations.Where(_ => _.UserId == id));
                context.SaveChanges();
                await userManager.DeleteAsync(user);
                var userDto = mapper.Map<ReadUserDto>(user);
                await hubContext.Clients.All.SendAsync("UserDeleted", userDto);
                return Ok(new ResponseAuth { Status = "Success", Message = $"User {user.Email} was deleted !" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseAuth { Status = "Error", Message = "Deleting user failed, user by this id does not exist!" });
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AddRolesForUser(string id, [FromBody] string[] roles)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (roles == null || roles.Length == 0) return BadRequest();
                var userRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, userRoles.ToArray());
                foreach (var role in roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                return Ok();
            }
            var userDto = mapper.Map<ReadUserDto>(user);
            await hubContext.Clients.All.SendAsync("UserEdited", userDto);
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

                var userDto = mapper.Map<ReadUserDto>(user);
                await hubContext.Clients.All.SendAsync("UserEdited", userDto);
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
