using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using team2backend.Data;
using team2backend.Models;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> hub;

        public SkillsController(ApplicationDbContext context, IHubContext<MessageHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            return Ok(await _context.Skills.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                var skill = await _context.Skills
                    .FirstOrDefaultAsync(m => m.Id == id);
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewSkill([FromBody] Skill skill)
        {
            if (ModelState.IsValid)
            {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            hub.Clients.All.SendAsync("SkillDeleted", skill);
            return Ok();
        }
    }
}
