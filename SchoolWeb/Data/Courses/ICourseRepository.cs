using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Courses
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<bool> IsCoursesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idCourse, string code);

        Task<Course> GetByCodeAsync(string code);

        IEnumerable<SelectListItem> GetComboCourses();
    }
}
