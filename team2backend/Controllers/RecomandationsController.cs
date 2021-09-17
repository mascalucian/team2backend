namespace team2backend.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using team2backend.Data;
    using team2backend.Models;

    /// <summary>
    ///   RecomandationController />.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RecomandationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> hub;

        /// <summary>Initializes a new instance of the <see cref="RecomandationsController" /> class.</summary>
        /// <param name="context">The context.</param>
        public RecomandationsController(ApplicationDbContext context, IHubContext<MessageHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        /// <summary>Gets the recomandations.</summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>
        ///   GetRecomandation ActionResult.
        /// </returns>
        [HttpGet("{SkillId}")]
        public async Task<IActionResult> GetRecomandations(int skillId)
        {
            var recomandations = _context.Recomandations.Where(sId => sId.SkillId == skillId)
                .Distinct().ToListAsync();
            return Ok(await recomandations);
        }

        /// <summary>Creates the recomandation.</summary>
        /// <param name="recomandation">The recomandation.</param>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>CreateRecomandation ActionResult.</returns>
        [HttpPost("SkillId")]
        public async Task<IActionResult> CreateRecomandation([FromBody] Recomandation recomandation, int skillId)
        {
            try
            {
                recomandation.SkillId = skillId;
                _context.Recomandations.Add(recomandation);
                await _context.SaveChangesAsync();

                // We don't use Put or Delete methods in our app so only this should broadcast.
                hub.Clients.All.SendAsync("RecommendationAdded", recomandation);
                return Ok(recomandation);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>Edits the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="recomandationUpdated">The recomandation updated.</param>
        /// <returns>The recomandation updated ActionResult.</returns>
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

        /// <summary>Deletes the recomandation.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Deletes the recomandation ActionResult.</returns>
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
