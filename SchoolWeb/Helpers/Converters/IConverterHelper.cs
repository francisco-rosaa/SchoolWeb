using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Courses;

namespace SchoolWeb.Helpers.Converters
{
    public interface IConverterHelper
    {
        CoursesViewModel CourseToViewModel(Course course);

        IQueryable<CoursesViewModel> CoursesToViewModels(IQueryable<Course> courses);
    }
}
