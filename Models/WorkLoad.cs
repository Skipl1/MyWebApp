using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class WorkLoad
    {
        public int Id { get; set; }
        public int AcademicProgramId { get; set; }
        public int Semester { get; set; }
        public int? Lectures { get; set; } // nullable
        public int? Labs { get; set; } // nullable
        public int? SelfStudy { get; set; } // nullable
        public int? IntermediateAssessment { get; set; } // nullable
        public string? AssessmentType { get; set; } // nullable

        // Navigation
        public virtual AcademicProgram AcademicProgram { get; set; } = null!;
        public virtual ICollection<Sections> Sections { get; set; } = new List<Sections>();
    }
}