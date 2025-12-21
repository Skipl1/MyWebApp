using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Discipline")]
    public class Discipline
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        public virtual ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
        public virtual ICollection<DisciplineTeacher> DisciplineTeachers { get; set; } = new List<DisciplineTeacher>();
    }
}