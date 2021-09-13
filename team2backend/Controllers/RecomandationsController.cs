using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using team2backend.Data;
using team2backend.Models;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecomandationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecomandationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{SkillId}")]
        public async Task<IActionResult> GetRecomandations(int SkillId)
        {
            var recomandations = _context.Recomandations.Where(sId => sId.SkillId == SkillId)
                .Distinct().ToListAsync();
            return Ok(await recomandations);
        }

        [HttpPost("SkillId")]
        public async Task<IActionResult> CreateRecomandation([FromBody] Recomandation recomandation, int SkillId)
        {
            try
            {
                recomandation.SkillId = SkillId;
                _context.Recomandations.Add(recomandation);
                await _context.SaveChangesAsync();
                return Ok(recomandation);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Recomandation recomandationUpdated)
        {
            var recomandationToUpdate = await _context.Recomandations.FindAsync(id);

            if (recomandationToUpdate != null)
            {
                recomandationToUpdate.Rating = recomandationUpdated.Rating;
                recomandationToUpdate.AuthorName = recomandationUpdated.AuthorName;
                recomandationToUpdate.Feedback = recomandationUpdated.Feedback;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecomandation(int id)
        {
            var recomandation = await _context.Recomandations.FindAsync(id);
            _context.Recomandations.Remove(recomandation);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
