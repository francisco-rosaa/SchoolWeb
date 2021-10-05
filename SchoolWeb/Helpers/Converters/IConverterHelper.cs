using System.Linq;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Absences;
using SchoolWeb.Models.Classes;
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

        ClassesViewModel ClassToClassesViewModel(Class clas);

        IQueryable<ClassesViewModel> ClassesToClassesViewModels(IQueryable<Class> classes);

        RegisterClassViewModel ClassToRegisterClassViewModel(Class clas);

        AbsenceDisciplinesViewModel AbsenceStudentsToDisciplinesViewModel(AbsenceStudentsViewModel model);
    }
}
