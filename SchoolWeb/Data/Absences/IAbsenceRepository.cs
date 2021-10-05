using System.Linq;
using System.Threading.Tasks;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Absences;

namespace SchoolWeb.Data.Absences
{
    public interface IAbsenceRepository : IGenericRepository<Absence>
    {
        Task<bool> IsAbsencesEmptyAsync();

        Task<IQueryable<StudentAbsence>> GetClassStudentAbsencesAsync(int classId, int disciplineId);
    }
}
