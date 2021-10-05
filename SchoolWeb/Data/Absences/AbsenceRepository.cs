using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Absences;

namespace SchoolWeb.Data.Absences
{
    public class AbsenceRepository : GenericRepository<Absence>, IAbsenceRepository
    {
        private readonly DataContext _context;
        private readonly IConfigurationRepository _configurationRepository;

        public AbsenceRepository(DataContext context, IConfigurationRepository configurationRepository)
            : base(context)
        {
            _context = context;
            _configurationRepository = configurationRepository;
        }

        public async Task<bool> IsAbsencesEmptyAsync()
        {
            return await _context.Absences.FirstOrDefaultAsync() == null ? true : false;
        }

        public async Task<IQueryable<StudentAbsence>> GetClassStudentAbsencesAsync(int classId, int disciplineId)
        {
            var students = Enumerable.Empty<StudentAbsence>().AsQueryable();
            var configuration = await _configurationRepository.GetConfigurations();

            await Task.Run(() =>
            {
                students = (
                from user in _context.Users
                join userRole in _context.UserRoles
                on user.Id equals userRole.UserId
                join role in _context.Roles
                on userRole.RoleId equals role.Id
                join classStudent in _context.ClassStudents
                on user.Id equals classStudent.UserId
                where role.Name == "Student" && classStudent.ClassId == classId
                orderby user.FirstName
                select new
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicturePath = user.ProfilePicturePath,
                    HoursAbsence = (
                        from absence in _context.Absences
                        where absence.UserId == user.Id && absence.ClassId == classId && absence.DisciplineId == disciplineId
                        select absence.Duration
                        ).Sum(),
                    HoursDiscipline = (
                        from discipline in _context.Disciplines
                        where discipline.Id == disciplineId
                        select discipline.Duration
                        ).FirstOrDefault()
                }
                ).Select(x => new StudentAbsence()
                {
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProfilePicturePath = x.ProfilePicturePath,
                    HoursAbsence = x.HoursAbsence,
                    PercentageAbsence = CalculatePercentage(x.HoursDiscipline, x.HoursAbsence),
                    Failed = CalculatePercentage(x.HoursDiscipline, x.HoursAbsence) >= configuration.MaxPercentageAbsence ? true : false
                });
            });
            return students;
        }

        private static int CalculatePercentage(int hoursDiscipline, int hoursAbsence)
        {
            if (hoursDiscipline == 0 && hoursAbsence == 0)
            {
                return 0;
            }

            double total = Convert.ToDouble(hoursDiscipline);
            double partial = Convert.ToDouble(hoursAbsence);
            double percentage = 100 / (total / partial);

            return Convert.ToInt32(percentage);
        }
    }
}
