using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress]
        public string Username { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }
    }
}
