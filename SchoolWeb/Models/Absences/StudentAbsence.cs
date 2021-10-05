using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models.Absences
{
    public class StudentAbsence
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Picture")]
        public string ProfilePicturePath { get; set; }

        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Hours")]
        public int HoursAbsence { get; set; }

        [Display(Name = "Percentage")]
        public int PercentageAbsence { get; set; }

        [Range(1, 100, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int? Duration { get; set; }

        public bool Failed { get; set; }
    }
}
