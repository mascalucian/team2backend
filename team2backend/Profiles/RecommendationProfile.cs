using AutoMapper;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Profiles
{
    public class RecommendationProfile : Profile
    {
        public RecommendationProfile()
        {
            CreateMap<Recomandation, GetRecommendationsByUserId>();

            CreateMap<Recomandation, GetRecommendationsBySkillId>();

            CreateMap<CreateRecommendationDto, Recomandation>();

            CreateMap<EditRecommendationDto, Recomandation>();
        }
    }
}
