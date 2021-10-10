using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Disciplines
{
    public interface IDisciplineRepository : IGenericRepository<Discipline>
    {
        Task<bool> IsDisciplinesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idDiscipline, string code);

        Task<Discipline> GetByCodeAsync(string code);

        Task<IEnumerable<SelectListItem>> GetComboDisciplinesInCourseAsync(int courseId);

        Task<IQueryable<HomeDisciplineViewModel>> GetHomeDisciplinesInCourseAsync(int courseId);
    }
}
