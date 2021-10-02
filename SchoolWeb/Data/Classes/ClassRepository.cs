﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

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
    }
}
