using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public class QualificationRepository : GenericRepository<Qualification>, IQualificationRepository
    {
        private readonly DataContext _context;

        public QualificationRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Qualification> GetQualificationByIdAsync(int id)
        {
            return await _context.Qualifications.FindAsync(id);
        }

        public async Task<Qualification> GetQualificationByNameAsync(string name)
        {
            return await _context.Qualifications.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task AddQualificationAsync(string name)
        {
            var qualification = await this.GetQualificationByNameAsync(name);

            if (qualification != null)
            {
                return;
            }

            await _context.Qualifications.AddAsync(new Qualification { Name = name });
            await _context.SaveChangesAsync();
        }

        public IEnumerable<SelectListItem> GetComboQualifications()
        {
            var list = _context.Qualifications.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select qualification...)",
                Value = "0"
            });

            return list;
        }
    }
}
