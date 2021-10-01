using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Models.Classes
{
    public class RegisterClassViewModel : Class
    {
        public IEnumerable<SelectListItem> Courses { get; set; }
    }
}
