using System.Threading.Tasks;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetConfigurations();

        Task<bool> SaveConfigurations(int maxStudents, int maxPercentAbsence);
    }
}
