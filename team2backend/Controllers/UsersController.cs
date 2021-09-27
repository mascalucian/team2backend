﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using team2backend.Authentication;
using team2backend.Dtos;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UsersController(UserManager<ApplicationUser> userManager,  IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
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

        [HttpPost("{id}")]
        public async Task<IActionResult> AddRolesForUser(string id, [FromBody] string[] roles)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                foreach (var role in roles)
                {
                    if (!await userManager.IsInRoleAsync(user, role))
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
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
    }
}
