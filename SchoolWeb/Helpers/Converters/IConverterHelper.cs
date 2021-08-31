using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Courses;
using SchoolWeb.Models.Disciplines;

namespace SchoolWeb.Helpers.Converters
{
    public interface IConverterHelper
    {
        CoursesViewModel CourseToCoursesViewModel(Course course);

        IQueryable<CoursesViewModel> CoursesToCoursesViewModels(IQueryable<Course> courses);

        DisciplinesViewModel DisciplineToDisciplinesViewModel(Discipline discipline);

        IQueryable<DisciplinesViewModel> DisciplinesToDisciplinesViewModels(IQueryable<Discipline> disciplines);
    }
}
