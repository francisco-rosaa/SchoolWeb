using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Data.Entities
{
    public class Class : IEntity
    {

        public int Id { get; set; }


        [MaxLength(20)]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{0} must be {2} characters minimum and {1} characters maximum")]
        public string Code { get; set; }


        [MaxLength(100)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be {2} characters minimum and {1} characters maximum")]
        public string Name { get; set; }


        [Display(Name = "Course")]
        [Required(ErrorMessage = "{0} is required")]
        public int CourseId { get; set; }

        public Course Course { get; set; }


        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }


        [Display(Name = "End Date")]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime EndDate { get; set; }
    }
}
