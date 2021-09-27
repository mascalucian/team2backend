using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using team2backend.Dtos;
using team2backend.Interfaces;
using team2backend.Models;

namespace team2backend.Controllers
{
    /// <summary>
    ///   RecomandationController />.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RecomandationsController : Controller
    {
        private readonly IHubContext<MessageHub> hub;
        private readonly IRecommendationsRepository recommendationRepository;

        /// <summary>Initializes a new instance of the <see cref="RecomandationsController" /> class.</summary>
        /// <param name="context">The context.</param>
        public RecomandationsController(IHubContext<MessageHub> hub, IRecommendationsRepository recommendationRepository)
        {
            this.hub = hub;
            this.recommendationRepository = recommendationRepository;
        }

        /// <summary>Gets the recomandations.</summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>
        ///   GetRecomandation ActionResult.
        /// </returns>
        [HttpGet("{skillId}")]
        public async Task<IActionResult> GetRecomandationsBySkillId(int skillId)
        {
            try
            {
                return Ok(recommendationRepository.GetRecomandationsBySkillId(skillId));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRecomandationsForUser(string userId)
        {
            try
            {
                return Ok(recommendationRepository.GetRecomandationsForUser(userId));
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>Creates the recomandation.</summary>
        /// <param name="recomandation">The recomandation.</param>
        /// <returns>CreateRecomandation ActionResult.</returns>
        //[Authorize(Roles = UserRoles.Expert)]
        [HttpPost]
        public async Task<IActionResult> CreateRecomandation([FromBody] Recomandation recomandation)
        {
            if (recomandation.Rating < 0 || recomandation.Rating > 5)
            {
                return BadRequest();
            }

            try
            {
                var response = recommendationRepository.CreateRecomandation(recomandation);

                // We don't use Put or Delete methods in our app so only this should broadcast.
                await hub.Clients.All.SendAsync("RecommendationAdded", response);
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
        //[Authorize(Roles = UserRoles.Expert)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditRecommendationDto recomandationUpdatedDto)
        {
            try
            {
                recommendationRepository.Edit(id, recomandationUpdatedDto);
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>Deletes the recomandation.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Deletes the recomandation ActionResult.</returns>
        //[Authorize(Roles = UserRoles.Expert)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecomandation(int id)
        {
            try
            {
                recommendationRepository.DeleteRecomandation(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
