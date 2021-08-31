using System.Linq;
using System.Threading.Tasks;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.CourseDisciplines;

namespace SchoolWeb.Data.CourseDisciplines
{
    public interface ICourseDisciplinesRepository : IGenericRepository<CourseDiscipline>
    {
        Task<bool> IsCourseDisciplinesEmptyAsync();

        Task<IQueryable<Discipline>> GetDisciplinesByCourseIdAsync(int courseId);

        Task<IQueryable<DisciplineSelectable>> GetAllDisciplinesSelectableAsync(int courseId);

        Task<CourseDiscipline> GetCourseDisciplineAsync(int courseId, int disciplineId);
    }
}
