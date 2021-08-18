using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Data.Entities
{
    public class Report : IEntity
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        public User User { get; set; }


        [MaxLength(100)]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be {2} characters minimum and {1} characters maximum")]
        public string Title { get; set; }


        [MaxLength(8000)]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(8000, MinimumLength = 2, ErrorMessage = "{0} must be {2} characters minimum and {1} characters maximum")]
        public string Message { get; set; }


        public bool? Solved { get; set; }


        [Display(Name = "Solved Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? SolvedDate { get; set; }
    }
}
