namespace MyWebApp.Models
{
    public class Specialty
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Direction { get; set; }
        public string? Qualification { get; set; }
        public int Duration { get; set; }
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
    }
}