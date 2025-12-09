namespace MyWebApp.Models
{
    public class TeacherAssignment
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int TeacherId { get; set; }

        // Navigation
        public virtual Department Department { get; set; } = null!;
        public virtual User Teacher { get; set; } = null!;
    }
}