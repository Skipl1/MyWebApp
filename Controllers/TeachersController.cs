using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Security.Claims;

public class TeachersController : Controller
{
    
    private readonly ApplicationDbContext _context;

    public TeachersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Вспомогательный метод для проверки прав заведующего кафедрой
    private async Task<bool> CheckIfHead(int departmentId)
    {
        if (!User.Identity.IsAuthenticated || User.Identity.Name == null)
        {
            return false;
        }

        // 1. Получаем текущего пользователя
        var currentUser = await _context.Users
            .FirstOrDefaultAsync(u => (u.Surname + " " + u.Name) == User.Identity.Name);

        if (currentUser == null)
        {
            return false;
        }

        // 2. Получаем кафедру вместе с заведующим
        var department = await _context.Departments
            .Include(d => d.Head)
            .FirstOrDefaultAsync(d => d.Id == departmentId);

        // 3. Сравниваем логины
        return department != null && department.Head != null && currentUser.Login == department.Head.Login;
    }


    // ========== СТРАНИЦА КАФЕДРЫ ==========
    public async Task<IActionResult> Index(int id)
    {
        var dept = await _context.Departments
            .Where(d => d.Id == id)
            .Include(d => d.Head)
            .Include(d => d.TeacherAssignments)
                .ThenInclude(ta => ta.Teacher)
            .FirstOrDefaultAsync();

        if (dept == null)
            return NotFound();

        // Подгружаем дисциплины преподавателей
        foreach (var ta in dept.TeacherAssignments)
        {
            ta.Teacher.DisciplineTeachers = await _context.DisciplineTeachers
                .Where(dt => dt.TeacherId == ta.TeacherId)
                .Include(dt => dt.Discipline)
                .ToListAsync();
        }

        return View(dept);
    }

    // ========== POST: Назначение дисциплины (AssignDiscipline) ==========
    // ========== POST: Назначение дисциплины (AssignDiscipline) ==========
    [HttpPost]
    public async Task<IActionResult> AssignDiscipline(int teacherId, int disciplineId, string participationType, int departmentId)
    {
        // ПРОВЕРКА ПРАВ: Только заведующий может назначать
        if (!await CheckIfHead(departmentId))
        {
            return Forbid();
        }

        // *** ИЗМЕНЕННАЯ ЛОГИКА ПРОВЕРКИ ***
        // Теперь проверяем, существует ли запись с точно таким же TeacherId, DisciplineId И ParticipationType.
        var exists = await _context.DisciplineTeachers
            .AnyAsync(dt => dt.TeacherId == teacherId 
                        && dt.DisciplineId == disciplineId
                        && dt.ParticipationType == participationType); // <-- Добавили проверку по типу
        
        if (!exists)
        {
            _context.DisciplineTeachers.Add(new DisciplineTeacher
            {
                TeacherId = teacherId,
                DisciplineId = disciplineId,
                ParticipationType = participationType
            });
            await _context.SaveChangesAsync();
        }
        // **********************************

        return RedirectToAction("Index", new { id = departmentId });
    }

    // ========== POST: Удаление дисциплины (RemoveDiscipline) ==========
    [HttpPost]
    public async Task<IActionResult> RemoveDiscipline(int disciplineTeacherId, int departmentId)
    {
        // ПРОВЕРКА ПРАВ: Только заведующий может удалять
        if (!await CheckIfHead(departmentId))
        {
            return Forbid();
        }
        
        var dt = await _context.DisciplineTeachers.FindAsync(disciplineTeacherId);
        if (dt != null)
        {
            _context.DisciplineTeachers.Remove(dt);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", new { id = departmentId });
    }
}