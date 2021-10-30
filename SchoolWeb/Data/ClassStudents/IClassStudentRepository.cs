using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.API.Students;
using SchoolWeb.Models.ClassStudents;

namespace SchoolWeb.Data.ClassStudents
{
    public interface IClassStudentRepository : IGenericRepository<ClassStudent>
    {
        Task<bool> IsClassStudentsEmptyAsync();

        Task<IQueryable<string>> GetStudentsIdsByClassIdAsync(int classId);

        Task<IEnumerable<ClassStudentsViewModel>> GetClassStudentsListAsync(IQueryable<string> studentsIds);

        Task<IQueryable<StudentsSelectable>> GetAllStudentsSelectableAsync(int classId);

        Task<ClassStudent> GetClassStudentAsync(int classId, string studentId);

        Task<int> GetClassStudentsTotalAsync(int classId);

        Task<IQueryable<StudentViewModel>> GetStudentsByClassCodeAsync(string classCode);
    }
}
