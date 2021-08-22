using System.Collections.Generic;
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

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogOutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task<string> GetRoleByIdAsync(string roleId);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<string> GetUserRoleAsync(string userId);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<bool> IsPasswordChangedAsync(string username);

        Task<bool> ConfirmPasswordChangedAsync(string userId);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<bool> IsEmailConfirmedAsync(string username);

        Task<User> GetUserByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<string> GetUserProfilePictureAsync(string userId);

        Task<string> GetUserPictureAsync(string userId);

        Task DeletePictureAsync(string picturePath);

        IEnumerable<SelectListItem> GetComboRoles();
    }
}
