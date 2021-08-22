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
                    var emailConfirmed = await _userHelper.IsEmailConfirmedAsync(model.Username);

                    if (emailConfirmed)
                    {
                        var passwordChanged = await _userHelper.IsPasswordChangedAsync(model.Username);

                        if (passwordChanged)
                        {
                            if (this.Request.Query.Keys.Contains("ReturnUrl"))
                            {
                                return Redirect(this.Request.Query["ReturnUrl"].First());
                            }

                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.ErrorTitle = "Password Not Changed";
                            ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";

                            await _userHelper.LogOutAsync();
                            return View("Error");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "Email Not Confirmed";
                        ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";

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

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterUser(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var model = new RegisterUserViewModel
            {
                Roles = _userHelper.GetComboRoles(),
                Genders = _genderRepository.GetComboGenders(),
                Qualifications = _qualificationRepository.GetComboQualifications()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    // Picture
                    string pictureName = string.Empty;

                    if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                    {
                        pictureName = Guid.NewGuid() + Path.GetExtension(model.ProfilePictureFile.FileName);

                        await SaveUploadedPicture(model.ProfilePictureFile, pictureName);
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
                        ProfilePicture = pictureName != string.Empty ? pictureName : null,
                        PasswordChanged = false
                    };

                    var resultAdd = await _userHelper.AddUserAsync(user, model.Password);

                    if (resultAdd != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to register user");
                        return View(model);
                    }

                    // Role
                    string role = await _userHelper.GetRoleByIdAsync(model.RoleId);
                    await AddUserToRoleAsync(user, role);

                    string message = "<span class=\"text-success\">Registration successful</span>";

                    // Email
                    Response response = await SendRegistrationEmailAsync(user, model.Email, model.FullName);

                    if (response.IsSuccess)
                    {
                        message += "<span class=\"text-success\">Email sent</span>";
                        return RedirectToAction("RegisterUser", "Account", new { message });
                    }

                    message += "<br />Failed to send email";
                    return RedirectToAction("RegisterUser", "Account", new { message });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email already registered");

                    model.Roles = _userHelper.GetComboRoles();
                    model.Genders = _genderRepository.GetComboGenders();
                    model.Qualifications = _qualificationRepository.GetComboQualifications();
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Staff")]
        public IActionResult RegisterStudent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var model = new RegisterStudentViewModel
            {
                Genders = _genderRepository.GetComboGenders(),
                Qualifications = _qualificationRepository.GetComboQualifications()
            };

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
                    string profilePictureName = string.Empty;
                    string pictureName = string.Empty;

                    if (model.PictureFile != null && model.PictureFile.Length > 0)
                    {
                        pictureName = Guid.NewGuid() + Path.GetExtension(model.PictureFile.FileName);

                        await SaveUploadedPicture(model.PictureFile, pictureName);

                        if (model.UseAsProfilePic)
                        {
                            profilePictureName = Guid.NewGuid() + Path.GetExtension(model.PictureFile.FileName);

                            await SaveUploadedPicture(model.PictureFile, profilePictureName);
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
                        ProfilePicture = model.UseAsProfilePic ? profilePictureName : null,
                        Picture = pictureName,
                        PasswordChanged = false
                    };

                    var resultAdd = await _userHelper.AddUserAsync(user, model.Password);

                    if (resultAdd != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to register student");
                        return View(model);
                    }

                    // Role
                    await AddUserToRoleAsync(user, "Student");

                    string message = "<span class=\"text-success\">Registration successful</span>";

                    // Email
                    Response response = await SendRegistrationEmailAsync(user, model.Email, model.FullName);

                    if (response.IsSuccess)
                    {
                        message += "<br /><span class=\"text-success\">Email sent</span>";
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

        private async Task SaveUploadedPicture(IFormFile picture, string pictureName)
        {
            var path = Path.Combine
                (
                    Directory.GetCurrentDirectory(),
                    "wwwroot\\images\\pictures",
                    pictureName
                );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }
        }

        private async Task AddUserToRoleAsync(User user, string role)
        {
            await _userHelper.AddUserToRoleAsync(user, role);

            var isUserInRole = await _userHelper.IsUserInRoleAsync(user, role);

            if (!isUserInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }
        }

        private async Task<Response> SendRegistrationEmailAsync(User user, string email, string fullName)
        {
            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

            string tokenLink = Url.Action
                (
                    "ActivateAccount",
                    "Account",
                    new { userid = user.Id, token = myToken },
                    protocol: HttpContext.Request.Scheme
                );

            Response response = _mailHelper.SendEmail
                (
                    email,
                    "Activate Account",
                    "<h3>Activate SchoolWeb Account</h3>" +
                    $"<p>Dear {fullName}, you are now registered at SchoolWeb.</p>" +
                    $"<p>Please click <a href=\"{tokenLink}\">here</a> to activate your account.</p>" +
                    "<p>Thank you.</p>"
                );

            return response;
        }

        public async Task<IActionResult> EditProfile(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = new EditProfileViewModel
            {
                Genders = _genderRepository.GetComboGenders(),
                Qualifications = _qualificationRepository.GetComboQualifications(),
                Role = await _userHelper.GetUserRoleAsync(user.Id)
            };

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.GenderId = user.GenderId;
                model.QualificationId = user.QualificationId;
                model.CcNumber = user.CcNumber;
                model.BirthDate = user.BirthDate;
                model.Address = user.Address;
                model.City = user.City;
                model.PhoneNumber = user.PhoneNumber;                
                model.ProfilePicturePath = user.ProfilePicturePath;
                model.PicturePath = user.PicturePath;
                model.Email = user.Email;

                TempData["SessionUserProfilePicture"] = user.ProfilePicturePath;
                TempData["SessionUserFirstName"] = user.FirstName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    string profilePictureName = string.Empty;
                    string pictureName = string.Empty;
                    string oldProfilePictureName = string.Empty;
                    string oldPictureName = string.Empty;

                    if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                    {
                        profilePictureName = Guid.NewGuid() + Path.GetExtension(model.ProfilePictureFile.FileName);

                        await SaveUploadedPicture(model.ProfilePictureFile, profilePictureName);

                        oldProfilePictureName = await _userHelper.GetUserProfilePictureAsync(user.Id);
                    }

                    if (model.PictureFile != null && model.PictureFile.Length > 0)
                    {
                        pictureName = Guid.NewGuid() + Path.GetExtension(model.PictureFile.FileName);

                        await SaveUploadedPicture(model.PictureFile, pictureName);

                        oldPictureName = await _userHelper.GetUserPictureAsync(user.Id);
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.GenderId = model.GenderId;
                    user.QualificationId = model.QualificationId;
                    user.CcNumber = model.CcNumber;
                    user.BirthDate = model.BirthDate;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.PhoneNumber = model.PhoneNumber;

                    if (!string.IsNullOrEmpty(profilePictureName))
                    {
                        user.ProfilePicture = profilePictureName;
                    }

                    if (!string.IsNullOrEmpty(pictureName))
                    {
                        user.Picture = pictureName;
                    }

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(oldProfilePictureName))
                        {
                            await _userHelper.DeletePictureAsync(oldProfilePictureName);
                        }

                        if (!string.IsNullOrEmpty(oldPictureName))
                        {
                            await _userHelper.DeletePictureAsync(oldPictureName);
                        }

                        message = "User profile updated";

                        if (!string.IsNullOrEmpty(profilePictureName))
                        {
                            message += "<br />Profile picture updated";
                        }

                        if (!string.IsNullOrEmpty(pictureName))
                        {
                            message += "<br />Student picture updated";
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return RedirectToAction("EditProfile", "Account", new { message });
        }

        public async Task<IActionResult> ActivateAccount(string userId, string token)
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

            bool isEmailConfirmed = await _userHelper.IsEmailConfirmedAsync(user.Email);

            if (!isEmailConfirmed)
            {
                var result = await _userHelper.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    return NotFound();
                }

            }

            ViewBag.MessageEmail = "Email confirmed";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateAccount(ActivateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if (user != null)
                {
                    string emailMessage = await _userHelper.IsEmailConfirmedAsync(user.Email) ? "Email confirmed" : "Email not confirmed";

                    var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                    var resetResult = await _userHelper.ResetPasswordAsync(user, myToken, model.Password);

                    if (resetResult.Succeeded)
                    {
                        var passChanged = await _userHelper.ConfirmPasswordChangedAsync(model.UserId);

                        if (passChanged)
                        {
                            ViewBag.Message = "<span class=\"text-success\">Password changed successfully</span>";
                            ViewBag.MessageEmail = emailMessage;
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "Error while trying to confirm password change";
                            ViewBag.MessageEmail = emailMessage;
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error while trying to change password";
                        ViewBag.MessageEmail = emailMessage;
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return View(model);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email not registered");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = Url.Action
                    (
                        "ResetPassword",
                        "Account",
                        new { token = myToken },
                        protocol: HttpContext.Request.Scheme
                    );

                Response response = _mailHelper.SendEmail
                    (
                        model.Email,
                        "Password Reset",
                        "<h3>SchoolWeb Password Reset</h3>" +
                        $"<p>Dear {user.FullName}, to reset your password click <a href = \"{link}\">here</a>.</p>" +
                        "<p>Thank you.</p>"
                    );

                if (response.IsSuccess)
                {
                    ViewBag.Message = "<span class=\"text-success\">Access your email account and follow the link to reset your password</span>";
                }

                return View();
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);

            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    ViewBag.Message = "<span class=\"text-success\">Password reset successful</span>";
                    return View();
                }

                ViewBag.Message = "Error while trying to reset password";
                return View(model);
            }

            ViewBag.Message = "User not found";
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
