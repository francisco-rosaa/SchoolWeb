using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Helpers;

namespace SchoolWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserHelper _userHelper;

        public HomeController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }


        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    TempData["SessionUserProfilePicture"] = user.ProfilePicturePath;
                    TempData["SessionUserFirstName"] = user.FirstName;
                }
            }

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
