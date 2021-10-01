using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

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
    }
}
