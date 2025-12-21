using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("WorkLoad")]
    public class WorkLoad
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("academic_program_id")]
        public int AcademicProgramId { get; set; }

        [Required, Column("semester")]
        public int Semester { get; set; }

        [Required, Column("lectures")]
        public int Lectures { get; set; } = 0;

        [Required, Column("labs")]
        public int Labs { get; set; } = 0;

        [Required, Column("self_study")]
        public int SelfStudy { get; set; } = 0;

        [Required, Column("intermediate_assessment")]
        public int IntermediateAssessment { get; set; } = 0;

        [Required, MaxLength(255), Column("assessment_type")]
        public string AssessmentType { get; set; } = null!;

        [ForeignKey("AcademicProgramId")]
        public virtual AcademicProgram AcademicProgram { get; set; } = null!;

        public virtual ICollection<Sections> Sections { get; set; } = new List<Sections>();
    }
}