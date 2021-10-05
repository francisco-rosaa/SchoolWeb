﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Data.Absences;
using SchoolWeb.Data.Classes;
using SchoolWeb.Data.Courses;
using SchoolWeb.Data.Disciplines;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers.Converters;
using SchoolWeb.Models.Absences;

namespace SchoolWeb.Controllers
{
    public class AbsencesController : Controller
    {
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly IConverterHelper _converterHelper;

        public AbsencesController
            (
                IAbsenceRepository absenceRepository,
                IClassRepository classRepository,
                ICourseRepository courseRepository,
                IDisciplineRepository disciplineRepository,
                IConverterHelper converterHelper
            )
        {
            _absenceRepository = absenceRepository;
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _disciplineRepository = disciplineRepository;
            _converterHelper = converterHelper;
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceClasses(int Id)
        {
            var model = new AbsenceClassesViewModel
            {
                ClassId = Id,
                Classes = await _classRepository.GetComboClasses()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceClasses(AbsenceClassesViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("RegisterAbsenceDisciplines", "Absences", new { Id = model.ClassId });
            }

            model.Classes = await _classRepository.GetComboClasses();
            return View(model);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceDisciplines(int Id, int disciplineId)
        {
            if (Id == 0)
            {
                return RedirectToAction("RegisterAbsenceClasses", "Absences");
            }

            var clas = await _classRepository.GetByIdAsync(Id);

            if (clas == null)
            {
                ViewBag.ErrorTitle = "No Class Found";
                ViewBag.ErrorMessage = "Class doesn't exist or there was an error";
                return View("Error");
            }

            var course = await _courseRepository.GetByIdAsync(clas.CourseId);

            if (course == null)
            {
                ViewBag.ErrorTitle = "No Course Found";
                ViewBag.ErrorMessage = "Course doesn't exist or there was an error";
                return View("Error");
            }

            var model = new AbsenceDisciplinesViewModel
            {
                ClassId = clas.Id,
                ClassName = $"{clas.Code}  |  {clas.Name}",
                CourseId = course.Id,
                CourseName = $"{course.Code}  |  {course.Name}",
                DisciplineId = disciplineId,
                Disciplines = await _disciplineRepository.GetComboDisciplinesInCourse(course.Id)
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceDisciplines(AbsenceDisciplinesViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("RegisterAbsenceStudents", "Absences", model);
            }

            model.Disciplines = await _disciplineRepository.GetComboDisciplinesInCourse(model.CourseId);
            return View(model);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceStudents(AbsenceDisciplinesViewModel modelInput)
        {
            if (!string.IsNullOrEmpty(modelInput.Message))
            {
                ViewBag.Message = modelInput.Message;
            }

            if (ModelState.IsValid)
            {
                var discipline = await _disciplineRepository.GetByIdAsync(modelInput.DisciplineId);

                if (discipline == null)
                {
                    ViewBag.ErrorTitle = "No Discipline Found";
                    ViewBag.ErrorMessage = "Discipline doesn't exist or there was an error";
                    return View("Error");
                }

                var model = new AbsenceStudentsViewModel
                {
                    ClassId = modelInput.ClassId,
                    ClassName = modelInput.ClassName,
                    CourseName = modelInput.CourseName,
                    DisciplineId = modelInput.DisciplineId,
                    DisciplineName = $"{discipline.Code}  |  {discipline.Name}  ({discipline.Duration}Hours)",
                    DisciplineDuration = discipline.Duration,
                    Students = (await _absenceRepository.GetClassStudentAbsencesAsync(modelInput.ClassId, modelInput.DisciplineId)).ToList()
                };

                return View(model);
            }

            return RedirectToAction("RegisterAbsenceClasses", "Absences");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterAbsenceStudents(AbsenceStudentsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var clas = await _classRepository.GetByIdAsync(model.ClassId);

                if (clas == null)
                {
                    ViewBag.ErrorTitle = "No Class Found";
                    ViewBag.ErrorMessage = "Class doesn't exist or there was an error";
                    return View("Error");
                }

                var discipline = await _disciplineRepository.GetByIdAsync(model.DisciplineId);

                if (discipline == null)
                {
                    ViewBag.ErrorTitle = "No Discipline Found";
                    ViewBag.ErrorMessage = "Discipline doesn't exist or there was an error";
                    return View("Error");
                }

                bool isDurationsNull = true;

                foreach (var student in model.Students)
                {
                    if (student.Duration != null)
                    {
                        isDurationsNull = false;
                        break;
                    }
                }

                var modelOut = _converterHelper.AbsenceStudentsToDisciplinesViewModel(model);

                if (isDurationsNull)
                {
                    modelOut.Message = "<span class=\"text-danger\">No absences registered</span>";
                    return RedirectToAction("RegisterAbsenceStudents", "Absences", modelOut);
                }

                bool isMaxHoursReached = false;

                try
                {
                    foreach (var student in model.Students)
                    {
                        if (student.Duration != null)
                        {
                            if (student.HoursAbsence < model.DisciplineDuration)
                            {
                                if ((student.HoursAbsence + student.Duration.Value) > model.DisciplineDuration)
                                {
                                    await _absenceRepository.CreateAsync(new Absence
                                    {
                                        UserId = student.UserId,
                                        ClassId = model.ClassId,
                                        DisciplineId = model.DisciplineId,
                                        Date = model.Date,
                                        Duration = model.DisciplineDuration - student.HoursAbsence
                                    });

                                    isMaxHoursReached = true;
                                }
                                else
                                {
                                    await _absenceRepository.CreateAsync(new Absence
                                    {
                                        UserId = student.UserId,
                                        ClassId = model.ClassId,
                                        DisciplineId = model.DisciplineId,
                                        Date = model.Date,
                                        Duration = student.Duration.Value
                                    });
                                }
                            }
                            else
                            {
                                isMaxHoursReached = true;
                            }
                        }
                    }
                }
                catch
                {
                    ViewBag.ErrorTitle = "DataBase Error";
                    ViewBag.ErrorMessage = "Error registering absences";
                    return View("Error");
                }

                if (isMaxHoursReached)
                {
                    modelOut.Message = "<span class=\"text-danger\">Maximum discipline hours reached for some student(s)</span>";
                }
                else
                {
                    modelOut.Message = "Absences registered successfully";
                }

                return RedirectToAction("RegisterAbsenceStudents", "Absences", modelOut);
            }

            return View(model);
        }
    }
}
