using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("Sections")]
    public class Sections
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("work_load_id")]
        public int WorkLoadId { get; set; }

        [Required, Column("index")]
        public int Index { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Required, Column("lecture_hours")]
        public int LectureHours { get; set; } = 0;

        [Required, Column("lab_hours")]
        public int LabHours { get; set; } = 0;

        [Required, Column("seminar_hours")]
        public int SeminarHours { get; set; } = 0;

        [Required, Column("self_study_hours")]
        public int SelfStudyHours { get; set; } = 0;

        [ForeignKey("WorkLoadId")]
        public virtual WorkLoad WorkLoad { get; set; } = null!;
    }
}