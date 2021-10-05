using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data.Classes
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<bool> IsClassesEmptyAsync();

        Task<bool> IsCodeInUseOnRegisterAsync(string code);

        Task<bool> IsCodeInUseOnEditAsync(int idClass, string code);

        Task<Class> GetByCodeAsync(string code);

        Task<IEnumerable<SelectListItem>> GetComboClasses();
    }
}
