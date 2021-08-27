using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers;
using SchoolWeb.Models;

namespace SchoolWeb.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;

        public ReportsController
            (
            IReportRepository reportRepository,
            IUserHelper userHelper,
            DataContext context
            )
        {
            _reportRepository = reportRepository;
            _userHelper = userHelper;
            _context = context;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndexReports(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    TempData["SessionUserProfilePicture"] = user.ProfilePicturePath;
                    TempData["SessionUserFirstName"] = user.FirstName;
                }
            }

            var reports = _reportRepository.GetAllReportsWithUsers();

            if (!reports.Any())
            {
                ViewBag.Message = "<span class=\"text-danger\">No Reports Found</span>";
            }

            return View(reports);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminReportDetails(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("AdminIndexReports", "Reports");
            }

            var report = await _reportRepository.GetReportByIdWithUserAsync(Id);

            if (report == null)
            {
                ViewBag.ErrorTitle = "No Report Found";
                ViewBag.ErrorMessage = "Report doesn't exist or there was an error";
                return View("Error");
            }

            return View(report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminReportDetails(ReportsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = await _context.Reports.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                if (report == null)
                {
                    ViewBag.ErrorTitle = "Report Not Found";
                    ViewBag.ErrorMessage = "Report doesn't exist or there was an error";
                    return View("Error");
                }

                var user = await _userHelper.GetUserByIdAsync(report.UserId);

                if (user == null)
                {
                    ViewBag.ErrorTitle = "User Not Found";
                    ViewBag.ErrorMessage = "User doesn't exist or there was an error";
                    return View("Error");
                }

                report.User = user;
                report.Solved = true;
                report.SolvedDate = DateTime.Today;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string message = "Report updated successfully";
                    return RedirectToAction("AdminIndexReports", "Reports", new { message });
                }
            }

            return RedirectToAction(nameof(AdminIndexReports));
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffIndexReports(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (user == null)
            {
                ViewBag.ErrorTitle = "User Not Found";
                ViewBag.ErrorMessage = "User doesn't exist or there was an error";
                return View("Error");
            }

            var reports = _reportRepository.GetAllReportsByUserAsync(user.Id);

            if (!reports.Any())
            {
                ViewBag.Message = "<span class=\"text-danger\">No Reports Found</span>";
            }

            return View(reports);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffReportDetails(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("StaffIndexReports", "Reports");
            }

            var report = await _reportRepository.GetReportByIdWithUserAsync(Id);

            if (report == null)
            {
                ViewBag.ErrorTitle = "No Report Found";
                ViewBag.ErrorMessage = "Report doesn't exist or there was an error";
                return View("Error");
            }

            return View(report);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffNewReport()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (user == null)
            {
                ViewBag.ErrorTitle = "User Not Found";
                ViewBag.ErrorMessage = "User doesn't exist or there was an error";
                return View("Error");
            }

            var model = new ReportsViewModel
            {
                UserId = user.Id
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffNewReport(ReportsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user == null)
                {
                    ViewBag.ErrorTitle = "User Not Found";
                    ViewBag.ErrorMessage = "User doesn't exist or there was an error";
                    return View("Error");
                }

                var report = new Report
                {
                    UserId = user.Id,
                    User = user,
                    Title = model.Title,
                    Message = model.Message,
                    Date = model.Date,
                    Solved = false
                };

                await _context.Reports.AddAsync(report);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string message = "Report added successfully";
                    return RedirectToAction("StaffIndexReports", "Reports", new { message });
                }
            }

            return View(model);
        }
    }
}
