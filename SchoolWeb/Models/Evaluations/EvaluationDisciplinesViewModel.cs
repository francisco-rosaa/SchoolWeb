using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolWeb.Models.Evaluations
{
    public class EvaluationDisciplinesViewModel
    {
        [Required]
        public int ClassId { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Display(Name = "Discipline")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Discipline")]
        [Required(ErrorMessage = "{0} is required")]
        public int DisciplineId { get; set; }

        public IEnumerable<SelectListItem> Disciplines { get; set; }

        public string Message { get; set; }
    }
}
