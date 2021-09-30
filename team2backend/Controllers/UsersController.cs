using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using team2backend.Authentication.Models;
using team2backend.Dtos;
using team2backend.Interfaces;
using team2backend.Models;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IHubContext<MessageHub> hubContext;
        private readonly IUsersRepository usersRepository;

        public UsersController(IHubContext<MessageHub> hubContext, IUsersRepository usersRepository)
        {
            this.hubContext = hubContext;
            this.usersRepository = usersRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await usersRepository.GetUserById(id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUserWithRoles([FromBody] AddNewUser user)
        {
            var newUserWithRoles = await usersRepository.AddNewUserWithRoles(user);
            if (newUserWithRoles.User == null)
            {
                return BadRequest(newUserWithRoles.Response);
            }

            var userDto = new ReadUserDto()
            {
                Id = newUserWithRoles.User.Id,
                UserName = newUserWithRoles.User.UserName,
                Email = newUserWithRoles.User.Email,
                Roles = newUserWithRoles.GoodRoles,
            };
            await hubContext.Clients.All.SendAsync("UserAdded", userDto);
            return Ok(newUserWithRoles.Response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await usersRepository.DeleteUser(id);
            if (user != null)
            {
                await hubContext.Clients.All.SendAsync("UserDeleted", user);
                return Ok(new ResponseAuth { Status = "Success", Message = $"User {user.Email} was deleted !" });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> EidtRolesForUser(string id, [FromBody] string[] roles)
        {
            if (roles.Length == 0)
            {
                return BadRequest();
            }

            var user = await usersRepository.EidtRolesForUser(id, roles);
            if (user != null)
            {
                var userDto = new ReadUserDto()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                };
                await hubContext.Clients.All.SendAsync("UserEdited", userDto);
                return Ok(userDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return Ok(await usersRepository.GetUsers());
            }
            catch (Exception)
            {
                return BadRequest();

            }
        }
    }
}
