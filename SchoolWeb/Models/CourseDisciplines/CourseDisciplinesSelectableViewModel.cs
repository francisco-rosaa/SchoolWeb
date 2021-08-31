using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SchoolWeb.Models.CourseDisciplines
{
    public class CourseDisciplinesSelectableViewModel
    {
        [Required]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        [Required]
        public IList<DisciplineSelectable> DisciplinesSelectable { get; set; }
    }
}
