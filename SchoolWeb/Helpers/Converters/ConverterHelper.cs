using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Courses;
using SchoolWeb.Models.Disciplines;

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

        public DisciplinesViewModel DisciplineToViewModel(Discipline discipline)
        {
            return new DisciplinesViewModel
            {
                Id = discipline.Id,
                Code = discipline.Code,
                Name = discipline.Name,
                Area = discipline.Area,
                Duration = discipline.Duration,
            };
        }

        public IQueryable<DisciplinesViewModel> DisciplinesToViewModels(IQueryable<Discipline> disciplines)
        {
            return disciplines.Select(x => new DisciplinesViewModel
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
