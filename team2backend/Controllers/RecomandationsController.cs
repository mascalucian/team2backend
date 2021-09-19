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
            var getRecommendationsBySkillId = mapper.Map<IEnumerable<GetRecommendationsBySkillId>>(recomandations);

            return Ok(getRecommendationsBySkillId);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRecomandationsForUser(string userId)
        {
            var recomandations = await _context.Recomandations.Where(recommendationId => recommendationId.UserId == userId)
                .Distinct().ToListAsync();
            var getRecommendationsByUserId = mapper.Map<IEnumerable<GetRecommendationsBySkillId>>(recomandations);
            return Ok(getRecommendationsByUserId);
        }

        /// <summary>Creates the recomandation.</summary>
        /// <param name="recomandation">The recomandation.</param>
        /// <returns>CreateRecomandation ActionResult.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRecomandation([FromBody] CreateRecommendationDto recomandationDto)
        {
            if (recomandationDto.Rating < 0 || recomandationDto.Rating > 5)
            {
                return BadRequest();
            }

            try
            {
                Recomandation recommendation = mapper.Map<Recomandation>(recomandationDto);
                _context.Recomandations.Add(recommendation);
                await _context.SaveChangesAsync();
                var skill = _context.Skills.Find(recomandationDto.SkillId);
                var response = new { recomandationDto, skill };

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
        public async Task<IActionResult> Edit(int id, [FromBody] Recomandation recomandationUpdated)
        {
            var recomandationToUpdate = await _context.Recomandations.FindAsync(id);

            if (recomandationToUpdate != null)
            {
                recomandationToUpdate.Rating = recomandationUpdated.Rating;
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
