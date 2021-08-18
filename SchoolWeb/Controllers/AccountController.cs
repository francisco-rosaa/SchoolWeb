using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Data;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers;
using SchoolWeb.Models;

namespace SchoolWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IGenderRepository _genderRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IMailHelper _mailHelper;

        public AccountController
            (
                IUserHelper userHelper,
                IGenderRepository genderRepository,
                IQualificationRepository qualificationRepository,
                IMailHelper mailHelper
            )
        {
            _userHelper = userHelper;
            _genderRepository = genderRepository;
            _qualificationRepository = qualificationRepository;
            _mailHelper = mailHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    var emailConfirmed = await _userHelper.IsEmailConfirmed(model.Username);

                    if (emailConfirmed)
                    {
                        if (this.Request.Query.Keys.Contains("ReturnUrl"))
                        {
                            return Redirect(this.Request.Query["ReturnUrl"].First());
                        }

                        return this.RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "Email Not Confirmed";
                        ViewBag.ErrorMessage = "Please enter your email account and follow the link to confirm your email";

                        await _userHelper.LogOutAsync();
                        return View("Error");
                    }
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<ActionResult> Logout()
        {
            await _userHelper.LogOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Staff")]
        public IActionResult RegisterStudent(string message)
        {
            var model = new RegisterStudentViewModel
            {
                Genders = _genderRepository.GetComboGenders(),
                Qualifications = _qualificationRepository.GetComboQualifications()
            };

            if(!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    // Picture
                    Guid pictureId = Guid.NewGuid();

                    if (model.PictureFile != null && model.PictureFile.Length > 0)
                    {
                        var path = Path.Combine
                            (
                                Directory.GetCurrentDirectory(),
                                "wwwroot\\images\\pictures",
                                pictureId.ToString()
                            );

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.PictureFile.CopyToAsync(stream);
                        }
                    }

                    // User
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        GenderId = model.GenderId,
                        QualificationId = model.QualificationId,
                        CcNumber = model.CcNumber,
                        BirthDate = model.BirthDate,
                        Address = model.Address,
                        City = model.City,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        UserName = model.Email,
                        ProfilePicture = model.UseAsProfilePic == true ? pictureId.ToString() : null,
                        Picture = pictureId.ToString()
                    };

                    var resultAdd = await _userHelper.AddUserAsync(user, model.Password);

                    if (resultAdd != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to register student");
                        return View(model);
                    }

                    // Role
                    await _userHelper.AddUserToRoleAsync(user, "Student");

                    var isUserInRole = await _userHelper.IsUserInRoleAsync(user, "Student");

                    if (!isUserInRole)
                    {
                        await _userHelper.AddUserToRoleAsync(user, "Student");
                    }

                    string message = "Registration successful";

                    // Email Token
                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    string tokenLink = Url.Action
                        (
                            "ConfirmEmail",
                            "Account",
                            new { userid = user.Id, token = myToken },
                            protocol: HttpContext.Request.Scheme
                        );

                    Response response = _mailHelper.SendEmail
                        (
                            model.Email,
                            "Email Confirmation",
                            "<h3>SchoolWeb Email Confirmation</h3>" +
                            $"<p>Dear {model.FullName}, you are now registered at SchoolWeb.<p>" +
                            $"<p>Please click <a href=\"{tokenLink}\">here</a> to confirm your email." +
                            "<p>Thank you.<p>"
                        );

                    if (response.IsSuccess)
                    {
                        message += "<br />Email sent";
                        return RedirectToAction("RegisterStudent", "Account", new { message });
                    }

                    message += "<br />Failed to send email";
                    return RedirectToAction("RegisterStudent", "Account", new { message });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email already registered");

                    model.Genders = _genderRepository.GetComboGenders();
                    model.Qualifications = _qualificationRepository.GetComboQualifications();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
