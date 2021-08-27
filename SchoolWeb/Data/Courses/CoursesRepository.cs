using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Courses
{
    public class CoursesRepository : GenericRepository<Course>, ICoursesRepository
    {
        private readonly DataContext _context;

        public CoursesRepository(DataContext context)
            : base (context)
        {
            _context = context;
        }
    }
}
