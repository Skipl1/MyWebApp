using Microsoft.EntityFrameworkCore;
using MyWebApp.Models; // Замени на имя твоего проекта
using System.Reflection; // Для получения свойств через рефлексию

namespace MyWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets — это "окна" в таблицы БД
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public DbSet<WorkLoad> WorkLoads { get; set; }
        public DbSet<Sections> Sections { get; set; }
        public DbSet<Curriculum> Curricula { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        public DbSet<DisciplineTeacher> DisciplineTeachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Укажем, что столбец 'Id' в C# соответствует столбцу 'id' в БД для всех сущностей
            // Это работает только для Primary Key (Id -> id)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var pk = entityType.FindPrimaryKey();
                if (pk != null && pk.Properties.Count == 1)
                {
                    var pkProperty = pk.Properties[0];
                    if (pkProperty.Name == "Id")
                    {
                        pkProperty.SetColumnName("id");
                    }
                }
            }

            // Настройка таблиц и столбцов для соответствия БД

            // Faculty
            modelBuilder.Entity<Faculty>()
                .ToTable("Faculty")
                .Property(f => f.Id).HasColumnName("id");
            modelBuilder.Entity<Faculty>()
                .Property(f => f.Name).HasColumnName("name");
            modelBuilder.Entity<Faculty>()
                .Property(f => f.Description).HasColumnName("description");

            // User
            modelBuilder.Entity<User>()
                .ToTable("User") // Важно: с заглавной буквы, как в БД
                .Property(u => u.Id).HasColumnName("id");
            modelBuilder.Entity<User>()
                .Property(u => u.Surname).HasColumnName("surname");
            modelBuilder.Entity<User>()
                .Property(u => u.Name).HasColumnName("name");
            modelBuilder.Entity<User>()
                .Property(u => u.Patronymic).HasColumnName("patronymic");
            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasColumnName("role");
            modelBuilder.Entity<User>()
                .Property(u => u.Login).HasColumnName("login");
            modelBuilder.Entity<User>()
                .Property(u => u.Password).HasColumnName("password");

            // Department
            modelBuilder.Entity<Department>()
                .ToTable("Department")
                .Property(d => d.Id).HasColumnName("id");
            modelBuilder.Entity<Department>()
                .Property(d => d.FacultyId).HasColumnName("faculty_id");
            modelBuilder.Entity<Department>()
                .Property(d => d.HeadId).HasColumnName("head_id"); // <-- Вот это важно
            modelBuilder.Entity<Department>()
                .Property(d => d.Name).HasColumnName("name");

            // Связь Department -> Faculty
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Faculty)
                .WithMany(f => f.Departments)
                .HasForeignKey(d => d.FacultyId); // Указываем имя столбца в БД

            // Связь Department -> User (Head)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Head)
                .WithMany()
                .HasForeignKey(d => d.HeadId); // ← используем свойство


            // Specialty
            modelBuilder.Entity<Specialty>()
                .ToTable("Specialty")
                .Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<Specialty>()
                .Property(s => s.DepartmentId).HasColumnName("department_id");
            modelBuilder.Entity<Specialty>()
                .Property(s => s.Name).HasColumnName("name");
            modelBuilder.Entity<Specialty>()
                .Property(s => s.Direction).HasColumnName("direction");
            modelBuilder.Entity<Specialty>()
                .Property(s => s.Qualification).HasColumnName("qualification");
            modelBuilder.Entity<Specialty>()
                .Property(s => s.Duration).HasColumnName("duration");

            // Связь Specialty -> Department
            modelBuilder.Entity<Specialty>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Specialties)
                .HasForeignKey(t => t.DepartmentId);


            // Discipline
            modelBuilder.Entity<Discipline>()
                .ToTable("Discipline")
                .Property(d => d.Id).HasColumnName("id");
            modelBuilder.Entity<Discipline>()
                .Property(d => d.Name).HasColumnName("name");

            // Curriculum
            modelBuilder.Entity<Curriculum>()
                .ToTable("Curriculum")
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Curriculum>()
                .Property(c => c.SpecialtyId).HasColumnName("specialty_id");
            modelBuilder.Entity<Curriculum>()
                .Property(c => c.DisciplineId).HasColumnName("discipline_id");
            modelBuilder.Entity<Curriculum>()
                .Property(c => c.Semester).HasColumnName("semester");
            modelBuilder.Entity<Curriculum>()
                .Property(c => c.CertificationType).HasColumnName("certification_type");

            // Связи Curriculum
            modelBuilder.Entity<Curriculum>()
                .HasOne(c => c.Specialty)
                .WithMany(s => s.Curricula)
                .HasForeignKey(dt => dt.SpecialtyId);
            modelBuilder.Entity<Curriculum>()
                .HasOne(c => c.Discipline)
                .WithMany(d => d.Curricula)
                .HasForeignKey(dt => dt.DisciplineId);

            // AcademicProgram
            modelBuilder.Entity<AcademicProgram>()
                .ToTable("AcademicProgram")
                .Property(ap => ap.Id).HasColumnName("id");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.SpecialtyId).HasColumnName("specialty_id");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.DisciplineId).HasColumnName("discipline_id");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Name).HasColumnName("name");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.StartYear).HasColumnName("start_year");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Status).HasColumnName("status");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Goals).HasColumnName("goals");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Competencies).HasColumnName("competencies");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Requirements).HasColumnName("requirements");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.DisciplinePosition).HasColumnName("discipline_position");
            modelBuilder.Entity<AcademicProgram>()
                .Property(ap => ap.Literature).HasColumnName("literature");

            // Связи AcademicProgram
            modelBuilder.Entity<AcademicProgram>()
                .HasOne(ap => ap.Specialty)
                .WithMany(s => s.AcademicPrograms)
                .HasForeignKey(dt => dt.SpecialtyId);
                
            modelBuilder.Entity<AcademicProgram>()
                .HasOne(ap => ap.Discipline)
                .WithMany(d => d.AcademicPrograms)
                .HasForeignKey(dt => dt.DisciplineId);

            // TeacherAssignment
            modelBuilder.Entity<TeacherAssignment>()
                .ToTable("TeacherAssignment")
                .Property(ta => ta.Id).HasColumnName("id");
            modelBuilder.Entity<TeacherAssignment>()
                .Property(ta => ta.DepartmentId).HasColumnName("department_id");
            modelBuilder.Entity<TeacherAssignment>()
                .Property(ta => ta.TeacherId).HasColumnName("teacher_id");

            // Связи TeacherAssignment
            modelBuilder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Department)
                .WithMany(d => d.TeacherAssignments)
                .HasForeignKey(t => t.DepartmentId);

            modelBuilder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Teacher) // У тебя это User
                .WithMany(u => u.TeacherAssignments)
                .HasForeignKey(dt => dt.TeacherId);


            // DisciplineTeacher
            modelBuilder.Entity<DisciplineTeacher>()
                .ToTable("DisciplineTeacher")
                .Property(dt => dt.Id).HasColumnName("id");
            modelBuilder.Entity<DisciplineTeacher>()
                .Property(dt => dt.TeacherId).HasColumnName("teacher_id");
            modelBuilder.Entity<DisciplineTeacher>()
                .Property(dt => dt.DisciplineId).HasColumnName("discipline_id");
            modelBuilder.Entity<DisciplineTeacher>()
                .Property(dt => dt.ParticipationType).HasColumnName("participation_type");

            // Связи DisciplineTeacher
            modelBuilder.Entity<DisciplineTeacher>()
                .HasOne(dt => dt.Teacher) // User
                .WithMany(u => u.DisciplineTeachers)
                .HasForeignKey(dt => dt.TeacherId);
            modelBuilder.Entity<DisciplineTeacher>()
                .HasOne(dt => dt.Discipline)
                .WithMany(d => d.DisciplineTeachers)
                .HasForeignKey(dt => dt.DisciplineId);


            // WorkLoad
            modelBuilder.Entity<WorkLoad>()
                .ToTable("WorkLoad")
                .Property(wl => wl.Id).HasColumnName("id");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.AcademicProgramId).HasColumnName("academic_program_id");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.Semester).HasColumnName("semester");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.Lectures).HasColumnName("lectures");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.Labs).HasColumnName("labs");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.SelfStudy).HasColumnName("self_study");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.IntermediateAssessment).HasColumnName("intermediate_assessment");
            modelBuilder.Entity<WorkLoad>()
                .Property(wl => wl.AssessmentType).HasColumnName("assessment_type");

            // Связь WorkLoad -> AcademicProgram
            modelBuilder.Entity<WorkLoad>()
                .HasOne(wl => wl.AcademicProgram)
                .WithMany(ap => ap.WorkLoads)
                .HasForeignKey(dt => dt.AcademicProgramId);


            // Sections
            modelBuilder.Entity<Sections>()
                .ToTable("Sections")
                .Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<Sections>()
                .Property(s => s.WorkLoadId).HasColumnName("work_load_id");
            modelBuilder.Entity<Sections>()
                .Property(s => s.Index).HasColumnName("index"); // "index" - ключевое слово, но PostgreSQL позволяет
            modelBuilder.Entity<Sections>()
                .Property(s => s.Name).HasColumnName("name");
            modelBuilder.Entity<Sections>()
                .Property(s => s.Description).HasColumnName("description");
            modelBuilder.Entity<Sections>()
                .Property(s => s.LectureHours).HasColumnName("lecture_hours");
            modelBuilder.Entity<Sections>()
                .Property(s => s.LabHours).HasColumnName("lab_hours");
            modelBuilder.Entity<Sections>()
                .Property(s => s.SeminarHours).HasColumnName("seminar_hours");
            modelBuilder.Entity<Sections>()
                .Property(s => s.SelfStudyHours).HasColumnName("self_study_hours");

            // Связь Sections -> WorkLoad
            modelBuilder.Entity<Sections>()
                .HasOne(s => s.WorkLoad)
                .WithMany(wl => wl.Sections)
                .HasForeignKey(dt => dt.WorkLoadId);
        }
    }
}