using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.Data.Entities;
using SchoolWeb.Models.Evaluations;

namespace SchoolWeb.Data.Evaluations
{
    public interface IEvaluationRepository : IGenericRepository<Evaluation>
    {
        Task<bool> IsEvaluationsEmptyAsync();

        Task<Evaluation> GetEvaluationAsync(string userId, int classId, int disciplineId);

        Task<IList<StudentsEvaluationIndexViewModel>> GetStudentEvaluationsIndexAsync();

        Task<IEnumerable<SelectListItem>> GetComboCoursesByStudent(string userId);

        Task<IEnumerable<EvaluationViewModel>> GetStudentEvaluationsByCourse(string userId, int courseId);

        Task<IQueryable<StudentEvaluation>> GetClassStudentEvaluationsAsync(int classId, int disciplineId);
    }
}
