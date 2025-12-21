using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Specialty")]
    public class Specialty
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("department_id")]
        public int DepartmentId { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Required, MaxLength(255), Column("direction")]
        public string Direction { get; set; } = null!;

        [Required, MaxLength(255), Column("qualification")]
        public string Qualification { get; set; } = null!;

        [Required, Column("duration")]
        public int Duration { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
    }
}