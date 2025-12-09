using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using Microsoft.EntityFrameworkCore;

public class FacultyController : Controller
{
    private readonly ApplicationDbContext _context;

    public FacultyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var faculties = await _context.Faculties.ToListAsync();
        return View(faculties);
    }
}
