using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Courses;

namespace SchoolWeb.Helpers.Converters
{
    public class ConverterHelper : IConverterHelper
    {
        public CoursesViewModel CourseToViewModel(Course course)
        {
            return new CoursesViewModel
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                Area = course.Area,
                Duration = course.Duration,
            };
        }

        public IQueryable<CoursesViewModel> CoursesToViewModels(IQueryable<Course> courses)
        {
            return courses.Select(x => new CoursesViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Area = x.Area,
                Duration = x.Duration,
            });
        }
    }
}
