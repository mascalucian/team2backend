namespace team2backend.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using team2backend.Data;
    using team2backend.Models;
    using team2backend.Services;

    /// <summary>
    ///   RecomandationController />.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RecomandationsController : Controller
    {
        private readonly RecommendationRepository repository;

        /// <summary>Initializes a new instance of the <see cref="RecomandationsController" /> class.</summary>
        /// <param name="context">The context.</param>
        public RecomandationsController(RecommendationRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>Gets the recomandations.</summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>
        ///   GetRecomandation ActionResult.
        /// </returns>
        [HttpGet("{SkillId}")]
        public async Task<IActionResult> GetRecomandations(int skillId)
        {
            return Ok(repository.GetRecomandationsForSkill(skillId));
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
                repository.Add(recomandation);
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
            var recomandationToUpdate = repository.Get(id);

            if (recomandationToUpdate != null)
            {
                repository.Update(id, recomandationUpdated);
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
            repository.Delete(id);
            return Ok();
        }
    }
}
