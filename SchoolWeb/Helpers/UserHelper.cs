﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models;

namespace SchoolWeb.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public UserHelper
            (
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                RoleManager<IdentityRole> roleManager,
                DataContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync
                (
                    model.Username,
                    model.Password,
                    model.RememberMe,
                    false
                );
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync( new IdentityRole {Name = roleName});
            }
        }

        public async Task<string> GetRoleByIdAsync(string roleId)
        {
            return await _context.Roles
                .Where(x => x.Id == roleId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<bool> IsPasswordChangedAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .Select(x => x.PasswordChanged)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> IsEmailConfirmedAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);

            return user.EmailConfirmed ? true : false;
        }

        public async Task<bool> ConfirmPasswordChangedAsync(string userId)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var passChanged = false;

            if (user != null)
            {
                user.PasswordChanged = true;
                await _context.SaveChangesAsync();
                passChanged = true;
            }

            return passChanged;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = _context.Roles
                .Where(x => x.Name != "Student")
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select role...)",
                Value = "0"
            });

            return list;
        }
    }
}