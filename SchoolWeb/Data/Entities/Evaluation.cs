using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Data.Entities
{
    public class Evaluation : IEntity
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        public User User { get; set; }


        [Display(Name = "Course")]
        [Required(ErrorMessage = "{0} is required")]
        public int CourseId { get; set; }

        public Course Course { get; set; }


        [Display(Name = "Discipline")]
        [Required(ErrorMessage = "{0} is required")]
        public int DisciplineId { get; set; }

        public Discipline Discipline { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [Range(0, 20, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Grade { get; set; }
    }
}
