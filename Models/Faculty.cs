using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Faculty")]
    public class Faculty
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}