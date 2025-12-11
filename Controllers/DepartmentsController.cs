using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

public class DepartmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int facultyId)
    {
        var faculty = await _context.Faculties
            .Include(f => f.Departments)
                .ThenInclude(d => d.Head)
            .FirstOrDefaultAsync(f => f.Id == facultyId);

        if (faculty == null)
            return NotFound();

        return View(faculty);
    }
}
