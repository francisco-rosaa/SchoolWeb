﻿using System.Collections.Generic;

namespace SchoolWeb.Models.Home
{
    public class HomeCoursesClassesViewModel
    {
        public IEnumerable<HomeCourseViewModel> Courses { get; set; }

        public IEnumerable<HomeClassViewModel> Classes { get; set; }
    }
}
