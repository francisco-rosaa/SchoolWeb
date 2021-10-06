using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWeb.Data.Classes;
using SchoolWeb.Data.ClassStudents;

namespace SchoolWeb.Controllers.API
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentsController : Controller
    {
        private readonly IClassRepository _classRepository;
        private readonly IClassStudentRepository _classStudentRepository;

        public StudentsController
            (
                IClassRepository classRepository,
                IClassStudentRepository classStudentRepository
            )
        {
            _classRepository = classRepository;
            _classStudentRepository = classStudentRepository;
        }


        [HttpGet]
        [Route("api/Students/GetStudentsByClass/{code}")]
        public async Task<IActionResult> GetStudentsByClass(string code)
        {
            var clas = await _classRepository.GetByCodeAsync(code);

            if (clas == null)
            {
                return NotFound($"Class with code '{code}' not found");
            }

            var students = await _classStudentRepository.GetStudentsByClassCodeAsync(code);

            if (!students.Any())
            {
                return NotFound($"Class '{code}' doesn't have any students");
            }

            return Ok(students);
        }
    }
}
