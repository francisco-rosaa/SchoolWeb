using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Data.Classes
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<bool> IsClassesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idClass, string code);

        Task<Class> GetByCodeAsync(string code);

        Task<IEnumerable<SelectListItem>> GetComboClassesAsync();

        Task<IQueryable<HomeClassViewModel>> GetHomeClassesAsync();
    }
}
