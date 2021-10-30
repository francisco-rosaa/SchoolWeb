using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Courses
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<bool> IsCoursesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idCourse, string code);

        Task<Course> GetByCodeAsync(string code);

        IEnumerable<SelectListItem> GetComboCourses();

        Task<IQueryable<HomeCourseViewModel>> GetHomeCoursesAsync();

        Task<IQueryable<Course>> GetStudentCourses(string userId);
    }
}
