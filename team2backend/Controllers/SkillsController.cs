using System.Linq;
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
        private readonly IUdemyCourseService udemyCourseService;

        public SkillsController(ISkillsRepository skillRepository, IHubContext<MessageHub> hub, IUdemyCourseService udemyCourseService)
        {
            this.skillRepository = skillRepository;
            this.hub = hub;
            this.udemyCourseService = udemyCourseService;
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
            var checkName = skillRepository.GetAllSkills().FirstOrDefault(_ => _.Name == skill.Name);
            if (checkName != null) return UnprocessableEntity();
            if (!udemyCourseService.HasResults(skill.Name)) return BadRequest();
            if (ModelState.IsValid)
            {
                await hub.Clients.All.SendAsync("SkillCreated", skill);
                skillRepository.CreateNewSkill(skill);
                return Ok();
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
                await hub.Clients.All.SendAsync("SkillUpdated", updatedSkill);
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
