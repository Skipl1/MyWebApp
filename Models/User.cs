using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("User")]
    public class User
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(255), Column("surname")]
        public string Surname { get; set; } = null!;

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Column("patronymic")]
        public string? Patronymic { get; set; }

        [Required, MaxLength(100), Column("role")]
        public string Role { get; set; } = null!;

        [Required, MaxLength(100), Column("login")]
        public string Login { get; set; } = null!;

        [Required, MaxLength(255), Column("password")]
        public string Password { get; set; } = null!;

        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
        public virtual ICollection<DisciplineTeacher> DisciplineTeachers { get; set; } = new List<DisciplineTeacher>();
    }
}