namespace team2backend.Models
{
    /// <summary>
    ///   Recomandation Model.
    /// </summary>
    public class Recomandation
    {
        public string Id { get; set; }

        public int CourseId { get; set; }

        public int Rating { get; set; }

        public string AuthorName { get; set; }

        public string Feedback { get; set; }

        // Navigation Properties
        public int SkillId { get; set; }
    }
}
