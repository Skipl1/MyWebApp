// MyWebApp/Models/Department.cs

using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public int FacultyId { get; set; }
        public int HeadId { get; set; } // <-- Это внешний ключ
        public string Name { get; set; } = null!;

        // Navigation
        public virtual Faculty Faculty { get; set; } = null!;
        public virtual User Head { get; set; } = null!; // <-- Навигационное свойство к User
        public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    }
}