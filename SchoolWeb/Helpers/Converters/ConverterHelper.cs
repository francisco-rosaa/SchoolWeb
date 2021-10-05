using System.Linq;
using SchoolWeb.Data.Courses;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Absences;
using SchoolWeb.Models.Classes;
using SchoolWeb.Models.Courses;
using SchoolWeb.Models.Disciplines;

namespace SchoolWeb.Helpers.Converters
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly ICourseRepository _courseRepository;

        public ConverterHelper(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public CoursesViewModel CourseToCoursesViewModel(Course course)
        {
            return new CoursesViewModel
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                Area = course.Area,
                Duration = course.Duration
            };
        }

        public IQueryable<CoursesViewModel> CoursesToCoursesViewModels(IQueryable<Course> courses)
        {
            return courses.Select(x => new CoursesViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Area = x.Area,
                Duration = x.Duration
            });
        }

        public DisciplinesViewModel DisciplineToDisciplinesViewModel(Discipline discipline)
        {
            return new DisciplinesViewModel
            {
                Id = discipline.Id,
                Code = discipline.Code,
                Name = discipline.Name,
                Area = discipline.Area,
                Duration = discipline.Duration
            };
        }

        public IQueryable<DisciplinesViewModel> DisciplinesToDisciplinesViewModels(IQueryable<Discipline> disciplines)
        {
            return disciplines.Select(x => new DisciplinesViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Area = x.Area,
                Duration = x.Duration
            });
        }

        public ClassesViewModel ClassToClassesViewModel(Class clas)
        {
            return new ClassesViewModel
            {
                Id = clas.Id,
                Code = clas.Code,
                Name = clas.Name,
                CourseId = clas.CourseId,
                Course = clas.Course,
                StartDate = clas.StartDate,
                EndDate = clas.EndDate
            };
        }

        public IQueryable<ClassesViewModel> ClassesToClassesViewModels(IQueryable<Class> classes)
        {
            return classes.Select(x => new ClassesViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                CourseId = x.CourseId,
                Course = x.Course,
                StartDate  = x.StartDate, 
                EndDate = x.EndDate
            });
        }

        public RegisterClassViewModel ClassToRegisterClassViewModel(Class clas)
        {
            return new RegisterClassViewModel
            {
                Id = clas.Id,
                Code = clas.Code,
                Name = clas.Name,
                CourseId = clas.CourseId,
                Courses = _courseRepository.GetComboCourses(),
                StartDate = clas.StartDate,
                EndDate = clas.EndDate
            };
        }

        public AbsenceDisciplinesViewModel AbsenceStudentsToDisciplinesViewModel(AbsenceStudentsViewModel model)
        {
            return new AbsenceDisciplinesViewModel
            {
                ClassId = model.ClassId,
                ClassName = model.ClassName,
                CourseId = model.CourseId,
                CourseName = model.CourseName,
                DisciplineId = model.DisciplineId
            };
        }
    }
}
