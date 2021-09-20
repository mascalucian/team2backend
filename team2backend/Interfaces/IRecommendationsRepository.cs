using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Interfaces
{
    public interface IRecommendationsRepository
    {
        object CreateRecomandation(Recomandation recomandation);

        void DeleteRecomandation(int id);

        void Edit(int id, EditRecommendationDto recomandationUpdatedDto);

        IEnumerable<Recomandation> GetRecomandationsBySkillId(int skillId);

        IEnumerable<Recomandation> GetRecomandationsForUser(string userId);
    }
}