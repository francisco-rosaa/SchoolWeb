using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Data;
using SchoolWeb.Helpers;
using SchoolWeb.Models.Configurations;

namespace SchoolWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfigurationRepository _configurationRepository;

        public HomeController(IUserHelper userHelper, IConfigurationRepository configurationRepository)
        {
            _userHelper = userHelper;
            _configurationRepository = configurationRepository;
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


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Configurations(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var configurations = await _configurationRepository.GetConfigurationsAsync();

            if (configurations == null)
            {
                ViewBag.ErrorTitle = "Configurations Not Found";
                ViewBag.ErrorMessage = "There was an error";
                return View("Error");
            }

            var model = new ConfigurationsViewModel
            {
                ClassMaxStudents = configurations.ClassMaxStudents,
                MaxPercentageAbsence = configurations.MaxPercentageAbsence
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Configurations(ConfigurationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _configurationRepository.SaveConfigurationsAsync(model.ClassMaxStudents, model.MaxPercentageAbsence);

                if (isSuccess)
                {
                    string message = "Configuration saved successfully";
                    return RedirectToAction("Configurations", "Home", new { message });
                }
            }

            return View(model);
        }
    }
}
