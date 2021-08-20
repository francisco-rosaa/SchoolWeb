using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IGenderRepository _genderRepository;
        private readonly IQualificationRepository _qualificationRepository;

        public SeedDb
            (
                DataContext context,
                IUserHelper userHelper,
                IGenderRepository genderRepository,
                IQualificationRepository qualificationRepository
            )
        {
            _context = context;
            _userHelper = userHelper;
            _genderRepository = genderRepository;
            _qualificationRepository = qualificationRepository;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await AddRolesAsync();
            await AddGendersAsync();
            await AddQualificationsAsync();
            await AddUserAdminAsync();
            await AddUserStaffAsync();
            await AddUserStudentAsync();
        }

        private async Task AddRolesAsync()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Staff");
            await _userHelper.CheckRoleAsync("Student");
        }

        private async Task AddGendersAsync()
        {
            await _genderRepository.AddGenderAsync("Male");
            await _genderRepository.AddGenderAsync("Female");
            await _genderRepository.AddGenderAsync("Not Specified");
        }

        private async Task AddQualificationsAsync()
        {
            await _qualificationRepository.AddQualificationAsync("Entry Level");
            await _qualificationRepository.AddQualificationAsync("Level 1");
            await _qualificationRepository.AddQualificationAsync("Level 2");
            await _qualificationRepository.AddQualificationAsync("Level 3");
            await _qualificationRepository.AddQualificationAsync("Level 4");
            await _qualificationRepository.AddQualificationAsync("Level 5");
            await _qualificationRepository.AddQualificationAsync("Level 6");
            await _qualificationRepository.AddQualificationAsync("Level 7");
            await _qualificationRepository.AddQualificationAsync("Level 8");
        }

        private async Task AddUserAdminAsync()
        {
            var userAdmin = await _userHelper.GetUserByEmailAsync("admin@gmail.com");

            if (userAdmin == null)
            {
                userAdmin = new User
                {
                    UserName = "admin@gmail.com",
                    FirstName = "James",
                    LastName = "Admin",
                    GenderId = _context.Genders.Where(x => x.Name == "Male").FirstOrDefault().Id,
                    QualificationId = _context.Qualifications.Where(x => x.Name == "Level 6").FirstOrDefault().Id,
                    CcNumber = "1234567890",
                    BirthDate = DateTime.Today.AddYears(-25),
                    Address = "Sunset Street, 7",
                    City = "Lisbon",
                    PhoneNumber = "+351210123456",
                    Email = "admin@gmail.com",
                    PasswordChanged = true
                };

                await AddUserWithRoleAsync(userAdmin, "Admin");
            }

            await IsUserInRole(userAdmin, "Admin");
        }

        private async Task AddUserStaffAsync()
        {
            var userStaff = await _userHelper.GetUserByEmailAsync("staff@gmail.com");

            if (userStaff == null)
            {
                userStaff = new User
                {
                    UserName = "staff@gmail.com",
                    FirstName = "Karen",
                    LastName = "Staff",
                    GenderId = _context.Genders.Where(x => x.Name == "Female").FirstOrDefault().Id,
                    QualificationId = _context.Qualifications.Where(x => x.Name == "Level 4").FirstOrDefault().Id,
                    CcNumber = "4567890123",
                    BirthDate = DateTime.Today.AddYears(-30),
                    Address = "Dark Alley, 9",
                    City = "Lisbon",
                    PhoneNumber = "+351210456789",
                    Email = "staff@gmail.com",
                    PasswordChanged = true
                };

                await AddUserWithRoleAsync(userStaff, "Staff");
            }

            await IsUserInRole(userStaff, "Staff");
        }

        private async Task AddUserStudentAsync()
        {
            var userStudent = await _userHelper.GetUserByEmailAsync("student@gmail.com");

            if (userStudent == null)
            {
                userStudent = new User
                {
                    UserName = "student@gmail.com",
                    FirstName = "Frank",
                    LastName = "Student",
                    GenderId = _context.Genders.Where(x => x.Name == "Not Specified").FirstOrDefault().Id,
                    QualificationId = _context.Qualifications.Where(x => x.Name == "Entry Level").FirstOrDefault().Id,
                    CcNumber = "7890123456",
                    BirthDate = DateTime.Today.AddYears(-20),
                    Address = "Sunshine Street, 1",
                    City = "Lisbon",
                    PhoneNumber = "+351210789123",
                    Email = "student@gmail.com",
                    PasswordChanged = true
                };

                await AddUserWithRoleAsync(userStudent, "Student");
            }

            await IsUserInRole(userStudent, "Student");
        }

        private async Task AddUserWithRoleAsync(User user, string role)
        {
            var result = await _userHelper.AddUserAsync(user, "!Admin1");

            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException($"Seeding could not create {role} user");
            }

            await _userHelper.AddUserToRoleAsync(user, role);

            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, myToken);
        }

        private async Task IsUserInRole(User user, string role)
        {
            var isUserInRole = await _userHelper.IsUserInRoleAsync(user, role);

            if (!isUserInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }
        }
    }
}
