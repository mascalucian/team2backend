using System.Collections.Generic;

namespace team2backend
{
    public class Response
    {
        public bool WasOverFullFiled { get; set; }

        public bool NoSearchFound { get; set; }

        public long NumberOfCoursesFound { get; set; }

        public IEnumerable<UdemyCourse> Courses { get; set; }
    }
}
