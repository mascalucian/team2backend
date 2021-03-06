namespace team2backend.Models
{
    /// <summary>
    ///   Recomandation Model.
    /// </summary>
    public class Recomandation
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public int Rating { get; set; }

        public string UserName { get; set; }

        public string Feedback { get; set; }

        // Navigation Properties
        public int SkillId { get; set; }

        public string SkillName { get; set; }

        public string UserId { get; set; }
    }
}
