namespace MyWebApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public int FacultyId { get; set; }
        public int HeadId { get; set; }
        public string Name { get; set; } = null!;

        public virtual Faculty Faculty { get; set; } = null!;
        public virtual User Head { get; set; } = null!;
        public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    }
}