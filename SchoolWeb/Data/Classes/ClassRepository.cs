using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Classes
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private readonly DataContext _context;

        public ClassRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsClassesEmptyAsync()
        {
            return await _context.Classes.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnRegisterAsync(string code)
        {
            var discipline = await _context.Classes.Where(x => x.Code == code).FirstOrDefaultAsync();

            return discipline != null ? true : false;
        }

        public async Task<bool> IsCodeInUseOnEditAsync(int idClass, string code)
        {
            var discipline = await _context.Classes.Where(x => x.Id != idClass && x.Code == code).FirstOrDefaultAsync();

            return discipline != null ? true : false;
        }

        public async Task<Class> GetByCodeAsync(string code)
        {
            return await _context.Classes.Where(x => x.Code == code).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetComboClassesAsync()
        {
            var list = new List<SelectListItem>();

            await Task.Run(() =>
            {
                list = _context.Classes
                    .Select(x => new SelectListItem
                    {
                        Text = $"{x.Code}  |  {x.Name}",
                        Value = x.Id.ToString()
                    })
                    .ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select class...)",
                    Value = "0"
                });
            });

            return list;
        }

        public async Task<IQueryable<HomeClassViewModel>> GetHomeClassesAsync()
        {
            var classes = Enumerable.Empty<HomeClassViewModel>().AsQueryable();

            await Task.Run(() =>
            {
                classes = _context.Classes
                    .Include(x => x.Course)
                    .Where(x => x.CourseId == x.Course.Id)
                    .OrderBy(x => x.Name)
                    .Select(x => new HomeClassViewModel
                    {
                        Name = x.Name,
                        Course = x.Course.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate
                    });
            });

            return classes;
        }
    }
}
