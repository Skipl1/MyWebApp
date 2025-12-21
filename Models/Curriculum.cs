using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Curriculum")]
    public class Curriculum
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("specialty_id")]
        public int SpecialtyId { get; set; }

        [Required, Column("discipline_id")]
        public int DisciplineId { get; set; }

        [Required, Column("semester")]
        public int Semester { get; set; }

        [Required, MaxLength(255), Column("certification_type")]
        public string CertificationType { get; set; } = null!;

        [ForeignKey("SpecialtyId")]
        public virtual Specialty Specialty { get; set; } = null!;

        [ForeignKey("DisciplineId")]
        public virtual Discipline Discipline { get; set; } = null!;
    }
}