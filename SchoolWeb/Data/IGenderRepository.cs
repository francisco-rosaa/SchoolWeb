using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public interface IGenderRepository : IGenericRepository<Gender>
    {
        Task<Gender> GetGenderByNameAsync(string name);

        Task AddGenderAsync(string name);

        IEnumerable<SelectListItem> GetComboGenders();
    }
}
