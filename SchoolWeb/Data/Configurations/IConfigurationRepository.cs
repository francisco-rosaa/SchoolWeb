using System.Threading.Tasks;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetConfigurationsAsync();

        Task<bool> SaveConfigurationsAsync(int maxStudents, int maxPercentAbsence);
    }
}
