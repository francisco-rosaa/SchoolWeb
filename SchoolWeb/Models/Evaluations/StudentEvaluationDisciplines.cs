using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models.Evaluations
{
    public class StudentEvaluationDisciplines
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        [Display(Name = "Absence")]
        public int HoursAbsence { get; set; }

        [Display(Name = "%")]
        public int PercentageAbsence { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? Date { get; set; }

        public int? Grade { get; set; }

        public bool FailedAbsence { get; set; }

        public bool FailedGrade { get; set; }

        public string BackgroundColor
        {
            get
            {
                if (FailedAbsence || FailedGrade)
                {
                    return "background-color: #FFF2F2";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
