using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Courses
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly DataContext _context;

        public CourseRepository(DataContext context)
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

        public IEnumerable<SelectListItem> GetComboCourses()
        {
            var list = _context.Courses.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select course...)",
                Value = "0"
            });

            return list;
        }

        public async Task<IQueryable<HomeCourseViewModel>> GetHomeCoursesAsync()
        {
            var courses = Enumerable.Empty<HomeCourseViewModel>().AsQueryable();

            await Task.Run(() =>
            {
                courses = _context.Courses
                    .OrderBy(x => x.Name)
                    .Select(x => new HomeCourseViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Area = x.Area,
                        Duration = x.Duration
                    });
            });

            return courses;
        }
    }
}
