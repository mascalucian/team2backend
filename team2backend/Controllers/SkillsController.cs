using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using team2backend.Data;
using team2backend.Models;
using team2backend.Services;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillsController : Controller
    {
        private readonly SkillRepository repository;

        public SkillsController(SkillRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            return Ok(repository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                var skill = repository.Get(id);
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
                repository.Add(skill);
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
            var skillToUpdate = repository.Get(id);

            if (skillToUpdate != null)
            {
                repository.Update(id, updatedSkill);
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
            repository.Delete(id);
            return Ok();
        }
    }
}
