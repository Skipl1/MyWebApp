using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("AcademicProgram")]
    public class AcademicProgram
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("specialty_id")]
        public int SpecialtyId { get; set; }

        [Required, Column("discipline_id")]
        public int DisciplineId { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Required, Column("start_year")]
        public int StartYear { get; set; }

        [Required, MaxLength(100), Column("status")]
        public string Status { get; set; } = "draft";

        [Column("goals")]
        public string? Goals { get; set; }

        [Column("competencies")]
        public string? Competencies { get; set; }

        [Column("requirements")]
        public string? Requirements { get; set; }

        [Column("discipline_position")]
        public string? DisciplinePosition { get; set; }

        [Column("literature")]
        public string? Literature { get; set; }

        [ForeignKey("SpecialtyId")]
        public virtual Specialty Specialty { get; set; } = null!;

        [ForeignKey("DisciplineId")]
        public virtual Discipline Discipline { get; set; } = null!;

        public virtual ICollection<WorkLoad> WorkLoads { get; set; } = new List<WorkLoad>();
    }
}