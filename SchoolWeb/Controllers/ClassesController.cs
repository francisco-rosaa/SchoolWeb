using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Classes;
using SchoolWeb.Data.Courses;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers.Converters;
using SchoolWeb.Models.Classes;

namespace SchoolWeb.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassRepository _classRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ICourseRepository _courseRepository;

        public ClassesController
            (
                IClassRepository classRepository,
                IConverterHelper converterHelper,
                ICourseRepository courseRepository
            )
        {
            _classRepository = classRepository;
            _converterHelper = converterHelper;
            _courseRepository = courseRepository;
        }


        [Authorize(Roles = "Staff")]
        public IActionResult StaffIndexClasses(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var models = Enumerable.Empty<ClassesViewModel>();

            var classes = _classRepository.GetAll();

            if (classes.Any())
            {
                models = (_converterHelper.ClassesToClassesViewModels(classes)).OrderBy(x => x.Code);
            }
            else
            {
                ViewBag.Message = "<span class=\"text-danger\">No Classes Found</span>";
            }

            return View(models);
        }


        [Authorize(Roles = "Staff")]
        public IActionResult RegisterClass()
        {
            var model = new RegisterClassViewModel
            {
                Courses = _courseRepository.GetComboCourses()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterClass(RegisterClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isCodeUsed = await _classRepository.IsCodeInUseOnRegisterAsync(model.Code);

                if (isCodeUsed)
                {
                    ViewBag.Message = "Code already in use by other class";

                    model.Courses = _courseRepository.GetComboCourses();
                    return View(model);
                }

                if (model.StartDate.Date > model.EndDate.Date)
                {
                    ViewBag.Message = "End date must be after start date";

                    model.Courses = _courseRepository.GetComboCourses();
                    return View(model);
                }

                var clas = new Class
                {
                    Code = model.Code,
                    Name = model.Name,
                    CourseId = model.CourseId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                };

                try
                {
                    await _classRepository.CreateAsync(clas);

                    string message = "Class added successfully";
                    return RedirectToAction("StaffIndexClasses", "Classes", new { message });
                }
                catch
                {
                    ViewBag.ErrorTitle = "Class Not Added";
                    ViewBag.ErrorMessage = "There was an error";
                    return View("Error");
                }
            }

            model.Courses = _courseRepository.GetComboCourses();
            return View(model);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> EditClass(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("StaffIndexClasses", "Classes");
            }

            var clas = await _classRepository.GetByIdAsync(Id);

            if (clas == null)
            {
                ViewBag.ErrorTitle = "No Class Found";
                ViewBag.ErrorMessage = "Class doesn't exist or there was an error";
                return View("Error");
            }

            var model = _converterHelper.ClassToRegisterClassViewModel(clas);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> EditClass(RegisterClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isCodeInUse = await _classRepository.IsCodeInUseOnEditAsync(model.Id, model.Code);

                if (isCodeInUse)
                {
                    ViewBag.Message = "<span class=\"text-danger\">Code already in use by other class</span>";

                    model.Courses = _courseRepository.GetComboCourses();
                    return View(model);
                }

                if (model.StartDate.Date > model.EndDate.Date)
                {
                    ViewBag.Message = "<span class=\"text-danger\">End date must be after start date</span>";

                    model.Courses = _courseRepository.GetComboCourses();
                    return View(model);
                }

                var clas = await _classRepository.GetByIdAsync(model.Id);

                if (clas != null)
                {
                    clas.Code = model.Code;
                    clas.Name = model.Name;
                    clas.CourseId = model.CourseId;
                    clas.StartDate = model.StartDate;
                    clas.EndDate = model.EndDate;

                    try
                    {
                        await _classRepository.UpdateAsync(clas);

                        ViewBag.Message = "Class saved successfully";

                        model.Courses = _courseRepository.GetComboCourses();
                        return View(model);
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.Message.Contains("unique"))
                        {
                            ViewBag.ErrorTitle = $"'{clas.Code}' In Use";
                            ViewBag.ErrorMessage = "Code is already in use";
                        }

                        return View("Error");
                    }
                }
            }

            model.Courses = _courseRepository.GetComboCourses();
            return View(model);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteClass(int Id)
        {
            if (Id == 0)
            {
                ViewBag.ErrorTitle = "Class Not Defined";
                ViewBag.ErrorMessage = "Error trying to delete class";
                return View("Error");
            }

            var clas = await _classRepository.GetByIdAsync(Id);

            if (clas == null)
            {
                ViewBag.ErrorTitle = "Class Not Found";
                ViewBag.ErrorMessage = "Error trying to delete class";
                return View("Error");
            }

            string message = string.Empty;

            try
            {
                await _classRepository.DeleteAsync(clas);
                message = "Class deleted successfully";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"'{clas.Name}' In Use";
                    ViewBag.ErrorMessage = "Cannot be deleted because it is in use by one or more records";
                }

                return View("Error");
            }

            return RedirectToAction("StaffIndexClasses", "Classes", new { message });
        }
    }
}
