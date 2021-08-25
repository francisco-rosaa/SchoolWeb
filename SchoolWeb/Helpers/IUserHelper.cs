﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models;

namespace SchoolWeb.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(string userId);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogOutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task CheckRoleAsync(string roleName);

        Task<string> GetRoleByIdAsync(string roleId);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<string> GetUserRoleAsync(string userId);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<bool> IsPasswordChangedAsync(string username);

        Task<bool> ConfirmPasswordChangedAsync(string userId);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<bool> IsEmailConfirmedAsync(string username);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<string> GetUserProfilePictureAsync(string userId);

        Task<string> GetUserPictureAsync(string userId);

        Task DeletePictureAsync(string picturePath);

        IEnumerable<SelectListItem> GetComboRoles();

        Task<IEnumerable<EditUsersViewModel>> GetUsersListAsync();

        Task<IEnumerable<EditUsersViewModel>> GetStudentsListAsync();

        Task DeleteUserAsync(User user);
    }
}
