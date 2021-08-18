using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Data.Entities
{
    [Keyless]
    public class Configuration
    {

        [Display(Name = "Class Max Students")]
        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int ClassMaxStudents { get; set; }


        [Display(Name = "Max Percentage Absence")]
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int MaxPercentageAbsence { get; set; }
    }
}
