using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using team2backend.Authentication;
using team2backend.Interfaces;
using team2backend.Models;
using team2backend.Services;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillsController : Controller
    {
        private readonly ISkillsRepository skillRepository;
        private readonly IHubContext<MessageHub> hub;

        public SkillsController(ISkillsRepository skillRepository, IHubContext<MessageHub> hub)
        {
            this.skillRepository = skillRepository;
            this.hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            return Ok(skillRepository.GetAllSkills());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                return Ok(skillRepository.GetSkillById(id));
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateNewSkill([FromBody] Skill skill)
        {
            if (skillRepository.CreateNewSkill(skill) == null) return BadRequest();
            if (ModelState.IsValid)
            {
                await hub.Clients.All.SendAsync("SkillCreated", skill);
                return Ok(skillRepository.CreateNewSkill(skill));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Skill updatedSkill)
        {
            if (skillRepository.Edit(id: id, updatedSkill: updatedSkill) != null)
            {
                await hub.Clients.All.SendAsync("SkillUpdated", skillRepository.Edit(id: id, updatedSkill: updatedSkill));
                return Ok(skillRepository.Edit(id: id, updatedSkill: updatedSkill));
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skillToBeDeleted = skillRepository.GetSkillById(id);
            skillRepository.Delete(id);
            await hub.Clients.All.SendAsync("SkillDeleted", skillToBeDeleted);
            return Ok();
        }
    }
}
