using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Dtos
{
    public class CreateRecommendationDto
    {
        public int CourseId { get; set; }

        public int Rating { get; set; }

        public string UserName { get; set; }

        public string Feedback { get; set; }

        public int SkillId { get; set; }

        public string UserId { get; set; }
    }
}
