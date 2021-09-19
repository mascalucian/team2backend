namespace team2backend.Dtos
{
    public class GetRecommendationsByUserId
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public int Rating { get; set; }

        public string UserName { get; set; }

        public string Feedback { get; set; }

        public string SkillId { get; set; }
    }
}
