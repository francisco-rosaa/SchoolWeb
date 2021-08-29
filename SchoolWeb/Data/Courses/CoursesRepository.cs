using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Courses
{
    public class CoursesRepository : GenericRepository<Course>, ICoursesRepository
    {
        private readonly DataContext _context;

        public CoursesRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsCoursesEmptyAsync()
        {
            return await _context.Courses.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnRegisterAsync(string code)
        {
            var course = await _context.Courses.Where(x => x.Code == code).FirstOrDefaultAsync();

            return course != null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnEditAsync(int idCourse, string code)
        {
            var course = await _context.Courses.Where(x => x.Id != idCourse && x.Code == code).FirstOrDefaultAsync();

            return course != null ? true : false;
        }

        public async Task<Course> GetByCodeAsync(string code)
        {
            return await _context.Courses.Where(x => x.Code == code).FirstOrDefaultAsync();
        }
    }
}
