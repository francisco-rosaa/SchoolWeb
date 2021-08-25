﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;

        public AccountController
            (
                IUserHelper userHelper,
                IGenderRepository genderRepository,
                IQualificationRepository qualificationRepository,
                IMailHelper mailHelper,
                IConfiguration configuration
            )
        {
            _userHelper = userHelper;
            _genderRepository = genderRepository;
            _qualificationRepository = qualificationRepository;
            _mailHelper = mailHelper;
            _configuration = configuration;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    var emailConfirmed = await _userHelper.IsEmailConfirmedAsync(model.Username);

                    if (!emailConfirmed)
                    {
                        ViewBag.ErrorTitle = "Email Not Confirmed";
                        ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";

                        await _userHelper.LogOutAsync();
                        return View("Error");
                    }

                    var passwordChanged = await _userHelper.IsPasswordChangedAsync(model.Username);

                    if (!passwordChanged)
                    {
                        ViewBag.ErrorTitle = "Password Not Changed";
                        ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";

                        await _userHelper.LogOutAsync();
                        return View("Error");
                    }

                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.ToString());
                }
            }

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
        [ValidateAntiForgeryToken]
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
                        message += "<br /><span class=\"text-success\">Email sent</span>";
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


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUsers()
        {
            var users = await _userHelper.GetUsersListAsync();

            return View(users);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> EditStudents()
        {
            var students = await _userHelper.GetStudentsListAsync();

            return View(students);
        }


        [Authorize]
        public async Task<IActionResult> EditProfile(string message, string Id)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (!string.IsNullOrEmpty(Id))
            {
                user = await _userHelper.GetUserByIdAsync(Id);
            }

            var model = new EditProfileViewModel
            {
                Genders = _genderRepository.GetComboGenders(),
                Qualifications = _qualificationRepository.GetComboQualifications(),
                Role = await _userHelper.GetUserRoleAsync(user.Id)
            };

            if (user != null)
            {
                model.UserId = user.Id;
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

                if (string.IsNullOrEmpty(Id) || user.UserName == this.User.Identity.Name)
                {
                    TempData["SessionUserProfilePicture"] = user.ProfilePicturePath;
                    TempData["SessionUserFirstName"] = user.FirstName;
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

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

                        oldProfilePictureName = user.ProfilePicture;
                    }

                    if (model.PictureFile != null && model.PictureFile.Length > 0)
                    {
                        pictureName = Guid.NewGuid() + Path.GetExtension(model.PictureFile.FileName);

                        await SaveUploadedPicture(model.PictureFile, pictureName);

                        oldPictureName = user.Picture;
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

                    if (model.RemoveProfilePicture)
                    {
                        oldProfilePictureName = await _userHelper.GetUserProfilePictureAsync(user.Id);

                        user.ProfilePicture = null;

                        if (!string.IsNullOrEmpty(oldProfilePictureName))
                        {
                            var newResponse = await _userHelper.UpdateUserAsync(user);

                            if (newResponse.Succeeded)
                            {
                                await _userHelper.DeletePictureAsync(oldProfilePictureName);

                                message += "<br />Profile picture updated";
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("EditProfile", "Account", new { message });
        }


        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> DeleteProfile(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                ViewBag.ErrorTitle = "User Not Defined";
                ViewBag.ErrorMessage = "Error trying to delete user";
                return View("Error");
            }

            var user = await _userHelper.GetUserByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorTitle = "User Not Found";
                ViewBag.ErrorMessage = "Error trying to delete user";
                return View("Error");
            }

            if (user.UserName == this.User.Identity.Name)
            {
                ViewBag.ErrorTitle = "Current User";
                ViewBag.ErrorMessage = "You cannot delete yourself";
                return View("Error");
            }

            try
            {
                await _userHelper.DeleteUserAsync(user);

                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await _userHelper.DeletePictureAsync(user.ProfilePicture);
                }

                if (!string.IsNullOrEmpty(user.Picture))
                {
                    await _userHelper.DeletePictureAsync(user.Picture);
                }

                if (this.User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(EditUsers));
                }

                if (this.User.IsInRole("Staff"))
                {
                    return RedirectToAction(nameof(EditStudents));
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{user.FullName} In Use";
                    ViewBag.ErrorMessage = $"{user.FullName} cannot be deleted because it is in use by one or more records";
                }

                return View("Error");
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> ActivateAccount(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorTitle = "User ID or Token Missing";
                ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";
                return View("Error");
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorTitle = "User Not Found";
                ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";
                return View("Error");
            }

            bool isEmailConfirmed = await _userHelper.IsEmailConfirmedAsync(user.Email);

            if (!isEmailConfirmed)
            {
                var result = await _userHelper.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    ViewBag.ErrorTitle = "Failed Email Confirmation";
                    ViewBag.ErrorMessage = "Access your email account and follow the link to activate your account";
                    return View("Error");
                }
            }

            ViewBag.MessageEmail = "Email confirmed";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
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


        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        string message = "Password changed successfully";

                        return RedirectToAction("EditProfile", "Account", new { message });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        
                        var token = new JwtSecurityToken
                            (
                                _configuration["Tokens:Issuer"],
                                _configuration["Tokens:Audience"],
                                claims,
                                expires: DateTime.UtcNow.AddDays(7),
                                signingCredentials: credentials
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }


        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
