using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public interface IQualificationRepository
    {
        Task<Qualification> GetQualificationByIdAsync(int id);

        Task<Qualification> GetQualificationByNameAsync(string name);

        Task AddQualificationAsync(string name);        

        IEnumerable<SelectListItem> GetComboQualifications();
    }
}
