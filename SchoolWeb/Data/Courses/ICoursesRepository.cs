using System.Threading.Tasks;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Courses
{
    public interface ICoursesRepository : IGenericRepository<Course>
    {
        Task<bool> IsCoursesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idCourse, string code);

        Task<Course> GetByCodeAsync(string code);
    }
}
