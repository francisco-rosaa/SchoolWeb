using System.Linq;
using System.Threading.Tasks;
using SchoolWeb.Models;

namespace SchoolWeb.Data.Entities
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<bool> IsReportsEmptyAsync();

        Task<IQueryable<ReportsViewModel>> GetAllReportsWithUsersAsync();

        Task<ReportsViewModel> GetReportByIdWithUserAsync(int Id);

        Task<IQueryable<ReportsViewModel>> GetAllReportsByUserAsync(string userId);
    }
}
