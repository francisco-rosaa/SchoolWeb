using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.CourseDisciplines;

namespace SchoolWeb.Data.CourseDisciplines
{
    public class CourseDisciplineRepository : GenericRepository<CourseDiscipline>, ICourseDisciplineRepository
    {
        private readonly DataContext _context;

        public CourseDisciplineRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsCourseDisciplinesEmptyAsync()
        {
            return await _context.CourseDisciplines.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<IQueryable<Discipline>> GetDisciplinesByCourseIdAsync(int courseId)
        {
            var disciplines = Enumerable.Empty<Discipline>().AsQueryable();

            await Task.Run(() =>
            {
                disciplines = _context.CourseDisciplines
                .Include(x => x.Discipline)
                .Where(x => x.CourseId == courseId)
                .Select(x => new Discipline
                {
                    Id = x.Discipline.Id,
                    Code = x.Discipline.Code,
                    Name = x.Discipline.Name,
                    Area = x.Discipline.Area,
                    Duration = x.Discipline.Duration
                });
            });

            return disciplines;
        }

        public async Task<IQueryable<DisciplineSelectable>> GetAllDisciplinesSelectableAsync(int courseId)
        {
            var disciplinesSelectable = Enumerable.Empty<DisciplineSelectable>().AsQueryable();

            await Task.Run(() =>
            {
                disciplinesSelectable =
                    (
                        from disciplines in _context.Disciplines
                        select disciplines
                    )
                    .Select(x => new DisciplineSelectable
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Area = x.Area,
                        Duration = x.Duration,
                        IsSelected =
                        (
                            from courseDisciplines in _context.CourseDisciplines
                            where courseDisciplines.CourseId == courseId
                            select courseDisciplines.DisciplineId
                        )
                        .Contains(x.Id) ? true : false
                    });
            });

            return disciplinesSelectable;
        }

        public async Task<CourseDiscipline> GetCourseDisciplineAsync(int courseId, int disciplineId)
        {
            return await _context.CourseDisciplines
                .Where(x => x.CourseId == courseId && x.DisciplineId == disciplineId)
                .FirstOrDefaultAsync();
        }
    }
}
