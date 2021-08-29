using System.Threading.Tasks;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Disciplines
{
    public interface IDisciplinesRepository : IGenericRepository<Discipline>
    {
        Task<bool> IsDisciplinesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idDiscipline, string code);

        Task<Discipline> GetByCodeAsync(string code);
    }
}
