using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models
{
    public class ActivateAccountViewModel
    {

        public string UserId { get; set; }


        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[#$@!%&*?._\-]).{6,}",
            ErrorMessage = "Six characters minimum containing lowercase, uppercase, digit and special characters")]
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password")]
        [Required(ErrorMessage = "{0} is required")]
        public string Confirm { get; set; }
    }
}
