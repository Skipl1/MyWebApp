using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("DisciplineTeacher")]
    public class DisciplineTeacher
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("teacher_id")]
        public int TeacherId { get; set; }

        [Required, Column("discipline_id")]
        public int DisciplineId { get; set; }

        [Required, MaxLength(255), Column("participation_type")]
        public string ParticipationType { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; } = null!;

        [ForeignKey("DisciplineId")]
        public virtual Discipline Discipline { get; set; } = null!;
    }
}