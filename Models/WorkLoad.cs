namespace MyWebApp.Models
{
    public class WorkLoad
    {
        public int Id { get; set; }
        public int AcademicProgramId { get; set; }
        public int Semester { get; set; }
        public int? Lectures { get; set; }
        public int? Labs { get; set; }
        public int? SelfStudy { get; set; }
        public int? IntermediateAssessment { get; set; }
        public string? AssessmentType { get; set; }

        public virtual AcademicProgram AcademicProgram { get; set; } = null!;
        public virtual ICollection<Sections> Sections { get; set; } = new List<Sections>();
    }
}