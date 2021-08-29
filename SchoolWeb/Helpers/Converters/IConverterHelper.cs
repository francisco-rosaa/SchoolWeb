using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Courses;
using SchoolWeb.Models.Disciplines;

namespace SchoolWeb.Helpers.Converters
{
    public interface IConverterHelper
    {
        CoursesViewModel CourseToViewModel(Course course);

        IQueryable<CoursesViewModel> CoursesToViewModels(IQueryable<Course> courses);

        DisciplinesViewModel DisciplineToViewModel(Discipline discipline);

        IQueryable<DisciplinesViewModel> DisciplinesToViewModels(IQueryable<Discipline> disciplines);
    }
}
