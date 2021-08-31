using SchoolWeb.Data.Entities;
using System.Collections.Generic;

namespace SchoolWeb.Models.CourseDisciplines
{
    public class CourseDisciplinesViewModel : Course
    {
        public string CodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Name))
                {
                    return $"{Code}  |  {Name}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public IEnumerable<Discipline> Disciplines { get; set; }
    }
}
