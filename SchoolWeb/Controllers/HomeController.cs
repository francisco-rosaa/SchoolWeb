using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Data;
using SchoolWeb.Data.Classes;
using SchoolWeb.Data.Courses;
using SchoolWeb.Data.Disciplines;
using SchoolWeb.Helpers;
using SchoolWeb.Models.Configurations;
using SchoolWeb.Models.Home;

namespace SchoolWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IClassRepository _classRepository;
        private readonly IDisciplineRepository _disciplineRepository;

        public HomeController
            (
                IUserHelper userHelper,
                IConfigurationRepository configurationRepository,
                ICourseRepository courseRepository,
                IClassRepository classRepository,
                IDisciplineRepository disciplineRepository
                
            )
        {
            _userHelper = userHelper;
            _configurationRepository = configurationRepository;
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _disciplineRepository = disciplineRepository;
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

            var model = new HomeCoursesClassesViewModel
            {
                Courses = await _courseRepository.GetHomeCoursesAsync(),
                Classes = await _classRepository.GetHomeClassesAsync()
            };

            return View(model);
        }


        public async Task<IActionResult> HomeCourseDisciplines(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new HomeCourseDisciplinesViewModel
            {
                Disciplines = await _disciplineRepository.GetHomeDisciplinesInCourseAsync(Id)
            };

            return View(model);
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
