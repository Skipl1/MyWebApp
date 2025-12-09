using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } // nullable

        // Navigation
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}