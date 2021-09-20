using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using team2backend.Authentication;
using team2backend.Data;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> hub;
        private readonly IMapper mapper;

        public SkillsController(ApplicationDbContext context, IHubContext<MessageHub> hub, IMapper mapper)
        {
            _context = context;
            this.hub = hub;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _context.Skills.ToListAsync();
            return Ok(skills);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                var skill = await _context.Skills
                    .FirstOrDefaultAsync(m => m.Id == id);
                var getSkillById = mapper.Map<GetSkillByIdDto>(skill);
                return Ok(getSkillById);
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
            if (ModelState.IsValid)
            {
                var checkSkill = await _context.Skills
                   .FirstOrDefaultAsync(m => m.Name == skill.Name);
                if (checkSkill != null) return BadRequest();
                var content = UdemyCourseController.GetSearchResults(skill.Name, 1);
                var json = JObject.Parse(content);
                var numberOfCoursesPerSearch = json.Value<long>("count");
                if (numberOfCoursesPerSearch == 0) return BadRequest();
                _context.Add(skill);
                await _context.SaveChangesAsync();
                hub.Clients.All.SendAsync("SkillCreated", skill);
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
            var skillToUpdate = await _context.Skills.FindAsync(id);

            if (skillToUpdate != null)
            {
                skillToUpdate.Name = updatedSkill.Name;
                await _context.SaveChangesAsync();
                hub.Clients.All.SendAsync("SkillUpdated", skillToUpdate);
                return Ok();
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
            var skill = await _context.Skills.FindAsync(id);
            _context.Skills.Remove(skill);
            _context.Recomandations.RemoveRange(_context.Recomandations.Where(_ => _.SkillId == id));
            await _context.SaveChangesAsync();
            hub.Clients.All.SendAsync("SkillDeleted", skill);
            return Ok();
        }
    }
}
