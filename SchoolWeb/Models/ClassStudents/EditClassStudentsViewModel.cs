using System.Collections.Generic;

namespace SchoolWeb.Models.ClassStudents
{
    public class EditClassStudentsViewModel
    {
        public int ClassId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Course { get; set; }

        public string ClassName => $"{Code}  |  {Name}  -  {Course}";

        public IEnumerable<ClassStudentsViewModel> Students { get; set; }
    }
}
