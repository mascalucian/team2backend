﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend
{
    public class UdemyCourse
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Price { get; set; }

        public string CourseImage { get; set; }

        public string Headline { get; set; }

        public Instructor Instructor { get; set; }
    }
}
