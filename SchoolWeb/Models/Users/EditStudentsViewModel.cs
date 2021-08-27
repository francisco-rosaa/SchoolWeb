using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models
{
    public class EditStudentsViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Picture")]
        public string ProfilePicture { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Age
        {
            get
            {
                int age = DateTime.Today.Year - BirthDate.Year;

                if (BirthDate > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                return $"{age} Years";
            }
        }

        public string Qualification { get; set; }

        public string City { get; set; }

        public string Email { get; set; }
    }
}
