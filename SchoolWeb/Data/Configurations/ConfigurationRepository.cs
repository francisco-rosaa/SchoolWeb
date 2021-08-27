using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly DataContext _context;

        public ConfigurationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Configuration> GetConfigurations()
        {
            return await _context.Configurations.FirstOrDefaultAsync();
        }

        public async Task<bool> SaveConfigurations(int maxStudents, int maxPercentAbsence)
        {
            bool isSuccess = false;

            var configurations = await _context.Configurations.FirstOrDefaultAsync();

            if (configurations != null)
            {
                configurations.ClassMaxStudents = maxStudents;
                configurations.MaxPercentageAbsence = maxPercentAbsence;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    isSuccess = true;
                }
            }

            return isSuccess;
        }
    }
}
