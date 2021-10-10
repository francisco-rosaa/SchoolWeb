using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Disciplines
{
    public class DisciplineRepository : GenericRepository<Discipline>, IDisciplineRepository
    {
        private readonly DataContext _context;

        public DisciplineRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsDisciplinesEmptyAsync()
        {
            return await _context.Disciplines.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnRegisterAsync(string code)
        {
            var discipline = await _context.Disciplines.Where(x => x.Code == code).FirstOrDefaultAsync();

            return discipline != null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnEditAsync(int idDiscipline, string code)
        {
            var discipline = await _context.Disciplines.Where(x => x.Id != idDiscipline && x.Code == code).FirstOrDefaultAsync();

            return discipline != null ? true : false;
        }

        public async Task<Discipline> GetByCodeAsync(string code)
        {
            return await _context.Disciplines.Where(x => x.Code == code).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetComboDisciplinesInCourseAsync(int courseId)
        {
            var list = new List<SelectListItem>();

            await Task.Run(() =>
            {
                list = _context.CourseDisciplines
                    .Include(x => x.Discipline)
                    .Where(x => x.CourseId == courseId)
                    .Select(x => new SelectListItem
                    {
                        Text = $"{x.Discipline.Code}  |  {x.Discipline.Name}",
                        Value = x.Id.ToString()
                    })
                    .ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select discipline...)",
                    Value = "0"
                });
            });

            return list;
        }

        public async Task<IQueryable<HomeDisciplineViewModel>> GetHomeDisciplinesInCourseAsync(int courseId)
        {
            var disciplines = Enumerable.Empty<HomeDisciplineViewModel>().AsQueryable();

            await Task.Run(() =>
            {
                disciplines = _context.CourseDisciplines
                    .Include(x => x.Discipline)
                    .Where(x => x.DisciplineId == x.Id && x.CourseId == courseId)
                    .OrderBy(x => x.Discipline.Name)
                    .Select(x => new HomeDisciplineViewModel
                    {
                        Name = x.Discipline.Name,
                        Area = x.Discipline.Area,
                        Duration = x.Discipline.Duration
                    });
            });

            return disciplines;
        }
    }
}
