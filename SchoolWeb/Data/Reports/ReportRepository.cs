using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Models;

namespace SchoolWeb.Data.Entities
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly DataContext _context;

        public ReportRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsReportsEmptyAsync()
        {
            return await _context.Reports.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<IQueryable<ReportsViewModel>> GetAllReportsWithUsersAsync()
        {
            var reports = Enumerable.Empty<ReportsViewModel>().AsQueryable();

            await Task.Run(() =>
            {
                reports = _context.Reports
                .Include(x => x.User)
                .OrderBy(x => x.Date)
                .Select(x => new ReportsViewModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    User = x.User,
                    Title = x.Title,
                    Message = x.Message,
                    Date = x.Date,
                    Solved = x.Solved,
                    SolvedDate = x.SolvedDate
                });
            });

            return reports;
        }

        public async Task<ReportsViewModel> GetReportByIdWithUserAsync(int Id)
        {
            var report = await _context.Reports
                .Include(x => x.User)
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                return null;
            }

            return new ReportsViewModel
            {
                Id = report.Id,
                UserId = report.UserId,
                User = report.User,
                Title = report.Title,
                Message = report.Message,
                Date = report.Date,
                Solved = report.Solved,
                SolvedDate = report.SolvedDate
            };
        }

        public async Task<IQueryable<ReportsViewModel>> GetAllReportsByUserAsync(string userId)
        {
            var reports = Enumerable.Empty<ReportsViewModel>().AsQueryable();

            await Task.Run(() =>
            {
                reports = _context.Reports
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Date)
                .Select(x => new ReportsViewModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    User = x.User,
                    Title = x.Title,
                    Message = x.Message,
                    Date = x.Date,
                    Solved = x.Solved,
                    SolvedDate = x.SolvedDate
                });
            });

            return reports;
        }
    }
}
