using SchoolWeb.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.Models.Courses
{
    public class CoursesViewModel : Course
    {
        [Display(Name = "Area")]
        public string AreaShort
        {
            get
            {
                if (!string.IsNullOrEmpty(Area))
                {
                    if (Area.Length > 35)
                    {
                        return $"{Area.Substring(0, 35)}...";
                    }
                    else
                    {
                        return Area;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
