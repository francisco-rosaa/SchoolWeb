using System;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Models
{
    public class ReportsViewModel : Report
    {
        public string Age
        {
            get
            {
                return $"{(DateTime.Today - Date).Days} Days";
            }
        }
    }
}
