using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
public class TeachersController : Controller
{

    private readonly ApplicationDbContext _context;

    public TeachersController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<bool> CheckIfHead(int departmentId)
    {
        if (!User.Identity.IsAuthenticated || User.Identity.Name == null)
        {
            return false;
        }


        var currentUser = await _context.Users
            .FirstOrDefaultAsync(u => (u.Surname + " " + u.Name) == User.Identity.Name);

        if (currentUser == null)
        {
            return false;
        }


        var department = await _context.Departments
            .Include(d => d.Head)
            .FirstOrDefaultAsync(d => d.Id == departmentId);


        return department != null && department.Head != null && currentUser.Login == department.Head.Login;
    }

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


        foreach (var ta in dept.TeacherAssignments)
        {
            ta.Teacher.DisciplineTeachers = await _context.DisciplineTeachers
                .Where(dt => dt.TeacherId == ta.TeacherId)
                .Include(dt => dt.Discipline)
                .ToListAsync();
        }

        return View(dept);
    }

    [HttpPost]
    public async Task<IActionResult> AssignDiscipline(int teacherId, int disciplineId, string participationType, int departmentId)
    {

        if (!await CheckIfHead(departmentId))
        {
            return Forbid();
        }

        var exists = await _context.DisciplineTeachers
            .AnyAsync(dt => dt.TeacherId == teacherId
                        && dt.DisciplineId == disciplineId
                        && dt.ParticipationType == participationType);

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


        return RedirectToAction("Index", new { id = departmentId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveDiscipline(int disciplineTeacherId, int departmentId)
    {

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