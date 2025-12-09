namespace MyWebApp.Models
{
    public class Curriculum
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public int DisciplineId { get; set; }
        public int Semester { get; set; }
        public string? CertificationType { get; set; } // nullable

        // Navigation
        public virtual Specialty Specialty { get; set; } = null!;
        public virtual Discipline Discipline { get; set; } = null!;
    }
}