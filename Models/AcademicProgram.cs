namespace MyWebApp.Models
{
    public class AcademicProgram
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public int DisciplineId { get; set; }
        public string Name { get; set; } = null!;
        public int StartYear { get; set; }
        public string Status { get; set; } = null!;
        public string? Goals { get; set; }
        public string? Competencies { get; set; }
        public string? Requirements { get; set; }
        public string? DisciplinePosition { get; set; }
        public string? Literature { get; set; }

        public virtual Specialty Specialty { get; set; } = null!;
        public virtual Discipline Discipline { get; set; } = null!;
        public virtual ICollection<WorkLoad> WorkLoads { get; set; } = new List<WorkLoad>();
    }
}