using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models.CourseDisciplines
{
    public class DisciplineSelectable
    {
        [Required]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Area { get; set; }

        public int Duration { get; set; }

        [Display(Name = "Selected")]
        public bool IsSelected { get; set; }
    }
}
