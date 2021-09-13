using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Models
{
    public class Recomandation
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public int Rating { get; set; }

        public string AuthorName { get; set; }

        public string Feedback { get; set; }

        //Navigation Properties
        public int SkillId { get; set; }

        public Skill Skill { get; set; }

    }
}
