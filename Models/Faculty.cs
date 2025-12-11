namespace MyWebApp.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}