namespace MyWebApp.Models
{
    public class Discipline
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
        public virtual ICollection<DisciplineTeacher> DisciplineTeachers { get; set; } = new List<DisciplineTeacher>();
    }
}