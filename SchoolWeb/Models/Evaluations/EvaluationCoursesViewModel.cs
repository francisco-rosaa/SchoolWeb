﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolWeb.Models.Evaluations
{
    public class EvaluationCoursesViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Display(Name = "Course")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Course")]
        [Required(ErrorMessage = "{0} is required")]
        public int CourseId { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }
    }
}
