using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolWeb.Controllers
{
    public class ClassStudentsController : Controller
    {
        [Authorize(Roles = "Staff")]
        public IActionResult StaffIndexClassStudents(int Id, string message)
        {
            if (Id == 0)
            {
                return RedirectToAction("StaffIndexClasses", "Classes");
            }

            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            return View();
        }
    }
}
