using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolWeb.Models.Evaluations
{
    public class EvaluationClassesViewModel
    {
        [Display(Name = "Class")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Class")]
        [Required(ErrorMessage = "{0} is required")]
        public int ClassId { get; set; }

        public IEnumerable<SelectListItem> Classes { get; set; }
    }
}
