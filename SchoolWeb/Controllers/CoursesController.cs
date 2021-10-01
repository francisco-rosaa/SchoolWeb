using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Courses;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers;
using SchoolWeb.Helpers.Converters;
using SchoolWeb.Models.Courses;

namespace SchoolWeb.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public CoursesController
            (
                ICourseRepository courseRepository,
                IUserHelper userHelper,
                IConverterHelper converterHelper
            )
        {
            _courseRepository = courseRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndexCourses(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var models = Enumerable.Empty<CoursesViewModel>();

            var courses = _courseRepository.GetAll();

            if (courses.Any())
            {
                models = (_converterHelper.CoursesToCoursesViewModels(courses)).OrderBy(x => x.Name);
            }
            else
            {
                ViewBag.Message = "<span class=\"text-danger\">No Courses Found</span>";
            }

            return View(models);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult RegisterCourse()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterCourse(CoursesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isCodeUsed = await _courseRepository.IsCodeInUseOnRegisterAsync(model.Code);

                if (isCodeUsed)
                {
                    ViewBag.Message = "<span class=\"text-danger\">Code already in use by other course</span>";
                    return View(model);
                }

                var course = new Course
                {
                    Code = model.Code,
                    Name = model.Name,
                    Area = model.Area,
                    Duration = model.Duration
                };

                try
                {
                    await _courseRepository.CreateAsync(course);

                    string message = "Course added successfully";
                    return RedirectToAction("AdminIndexCourses", "Courses", new { message });
                }
                catch
                {
                    ViewBag.ErrorTitle = "Course Not Added";
                    ViewBag.ErrorMessage = "There was an error";
                    return View("Error");
                }
            }

            return View(model);
        }
        

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCourse(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("AdminIndexCourses", "Courses");
            }

            var course = await _courseRepository.GetByIdAsync(Id);

            if (course == null)
            {
                ViewBag.ErrorTitle = "No Course Found";
                ViewBag.ErrorMessage = "Course doesn't exist or there was an error";
                return View("Error");
            }

            var model = _converterHelper.CourseToCoursesViewModel(course);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCourse(CoursesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isCodeInUse = await _courseRepository.IsCodeInUseOnEditAsync(model.Id, model.Code);

                if (isCodeInUse)
                {
                    ViewBag.Message = "<span class=\"text-danger\">Code already in use by other course</span>";
                    return View(model);
                }

                var course = await _courseRepository.GetByIdAsync(model.Id);

                if (course != null)
                {
                    course.Code = model.Code;
                    course.Name = model.Name;
                    course.Area = model.Area;
                    course.Duration = model.Duration;

                    try
                    {
                        await _courseRepository.UpdateAsync(course);

                        ViewBag.Message = "Course saved successfully";
                        return View(model);
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.Message.Contains("unique"))
                        {
                            ViewBag.ErrorTitle = $"'{course.Code}' In Use";
                            ViewBag.ErrorMessage = "Code is already in use";
                        }

                        return View("Error");
                    }
                }
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int Id)
        {
            if (Id == 0)
            {
                ViewBag.ErrorTitle = "Course Not Defined";
                ViewBag.ErrorMessage = "Error trying to delete course";
                return View("Error");
            }

            var course = await _courseRepository.GetByIdAsync(Id);
  
            if (course == null)
            {
                ViewBag.ErrorTitle = "Course Not Found";
                ViewBag.ErrorMessage = "Error trying to delete course";
                return View("Error");
            }

            string message = string.Empty;

            try
            {
                await _courseRepository.DeleteAsync(course);
                message = "Course deleted successfully";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"'{course.Name}' In Use";
                    ViewBag.ErrorMessage = "Cannot be deleted because it is in use by one or more records";
                }

                return View("Error");
            }

            return RedirectToAction("AdminIndexCourses", "Courses", new { message });
        }
    }
}
