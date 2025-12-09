namespace MyWebApp.Models
{
    public class DisciplineTeacher
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int DisciplineId { get; set; }
        public string? ParticipationType { get; set; } // nullable

        // Navigation
        public virtual User Teacher { get; set; } = null!;
        public virtual Discipline Discipline { get; set; } = null!;
    }
}