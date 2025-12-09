using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class Specialty
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Direction { get; set; } // nullable
        public string? Qualification { get; set; } // nullable
        public int Duration { get; set; } // nullable, можно сделать int, если числовое

        // Navigation
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
    }
}