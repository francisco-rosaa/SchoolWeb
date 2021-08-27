using System.Linq;
using System.Threading.Tasks;
using SchoolWeb.Models;

namespace SchoolWeb.Data.Entities
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<bool> IsReportsEmptyAsync();

        IQueryable<ReportsViewModel> GetAllReportsWithUsers();

        Task<ReportsViewModel> GetReportByIdWithUserAsync(int Id);

        IQueryable<ReportsViewModel> GetAllReportsByUserAsync(string userId);
    }
}
