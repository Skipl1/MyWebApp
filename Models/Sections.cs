namespace MyWebApp.Models
{
    public class Sections
    {
        public int Id { get; set; }
        public int WorkLoadId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? LectureHours { get; set; }
        public int? LabHours { get; set; }
        public int? SeminarHours { get; set; }
        public int? SelfStudyHours { get; set; }

        public virtual WorkLoad WorkLoad { get; set; } = null!;
    }
}