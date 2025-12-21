using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("TeacherAssignment")]
    public class TeacherAssignment
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("department_id")]
        public int DepartmentId { get; set; }

        [Required, Column("teacher_id")]
        public int TeacherId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; } = null!;
    }
}