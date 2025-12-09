namespace MyWebApp.Models
{
    public class Sections
    {
        public int Id { get; set; }
        public int WorkLoadId { get; set; }
        public int Index { get; set; } // индекс раздела
        public string Name { get; set; } = null!;
        public string? Description { get; set; } // nullable
        public int? LectureHours { get; set; } // nullable
        public int? LabHours { get; set; } // nullable
        public int? SeminarHours { get; set; } // nullable
        public int? SelfStudyHours { get; set; } // nullable

        // Navigation
        public virtual WorkLoad WorkLoad { get; set; } = null!;
    }
}