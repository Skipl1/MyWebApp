using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class AcademicProgram
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public int DisciplineId { get; set; }
        public string Name { get; set; } = null!;
        public int StartYear { get; set; }
        public string Status { get; set; } = null!; // "draft", "approved", etc.
        public string? Goals { get; set; } // nullable
        public string? Competencies { get; set; } // nullable
        public string? Requirements { get; set; } // nullable
        public string? DisciplinePosition { get; set; } // nullable
        public string? Literature { get; set; } // nullable

        // Navigation
        public virtual Specialty Specialty { get; set; } = null!;
        public virtual Discipline Discipline { get; set; } = null!;
        public virtual ICollection<WorkLoad> WorkLoads { get; set; } = new List<WorkLoad>();
    }
}