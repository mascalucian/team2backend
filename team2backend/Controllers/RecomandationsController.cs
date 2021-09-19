namespace team2backend.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using team2backend.Data;
    using team2backend.Dtos;
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
        private readonly IMapper mapper;

        /// <summary>Initializes a new instance of the <see cref="RecomandationsController" /> class.</summary>
        /// <param name="context">The context.</param>
        public RecomandationsController(ApplicationDbContext context, IHubContext<MessageHub> hub, IMapper mapper)
        {
            _context = context;
            this.hub = hub;
            this.mapper = mapper;
        }

        /// <summary>Gets the recomandations.</summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>
        ///   GetRecomandation ActionResult.
        /// </returns>
        [HttpGet("{skillId}")]
        public async Task<IActionResult> GetRecomandationsBySkillId(int skillId)
        {
            var recomandations = await _context.Recomandations.Where(sId => sId.SkillId == skillId)
                .Distinct().ToListAsync();
            return Ok(recomandations);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRecomandationsForUser(string userId)
        {
            var recomandations = await _context.Recomandations.Where(recommendationId => recommendationId.UserId == userId)
                .Distinct().ToListAsync();
            return Ok(recomandations);
        }

        /// <summary>Creates the recomandation.</summary>
        /// <param name="recomandation">The recomandation.</param>
        /// <returns>CreateRecomandation ActionResult.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRecomandation([FromBody] Recomandation recomandation)
        {
            if (recomandation.Rating < 0 || recomandation.Rating > 5)
            {
                return BadRequest();
            }

            try
            {
                var skill = await _context.Skills
                    .FirstOrDefaultAsync(_ => _.Id == recomandation.SkillId);
                recomandation.SkillName = skill.Name;
                _context.Recomandations.Add(recomandation);
                await _context.SaveChangesAsync();
                var response = new { recomandation, skill };
                // We don't use Put or Delete methods in our app so only this should broadcast.
                hub.Clients.All.SendAsync("RecommendationAdded", response);
                return Ok();
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
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditRecommendationDto recomandationUpdatedDto)
        {
            var recomandationToUpdate = await _context.Recomandations.FindAsync(id);

            if (recomandationToUpdate != null)
            {
                var recommendation = mapper.Map<Recomandation>(recomandationUpdatedDto);
                recomandationToUpdate.Rating = recommendation.Rating;
                recomandationToUpdate.Feedback = recommendation.Feedback;
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
        [Authorize]
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
