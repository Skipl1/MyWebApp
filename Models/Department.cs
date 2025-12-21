using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Department")]
    public class Department
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("faculty_id")]
        public int FacultyId { get; set; }

        [Column("head_id")]
        public int? HeadId { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [ForeignKey("FacultyId")]
        public virtual Faculty Faculty { get; set; } = null!;

        [ForeignKey("HeadId")]
        public virtual User? Head { get; set; }

        public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    }
}