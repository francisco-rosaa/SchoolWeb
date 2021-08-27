using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Data.Entities
{
    public class Configuration : IEntity
    {

        public int Id { get; set; }


        [Display(Name = "Class Max Students")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(0, 1000, ErrorMessage = "Must be between {1} and {2}.")]
        public int ClassMaxStudents { get; set; }


        [Display(Name = "Max Percentage Absence")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(0, 100, ErrorMessage = "Must be between {1} and {2}.")]
        public int MaxPercentageAbsence { get; set; }
    }
}
