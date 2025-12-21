using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using System.Linq;

public class SpecialtiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpecialtiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var specialties = await _context.Specialties.ToListAsync();

        var groupedByDirection = specialties
            .Where(s => !string.IsNullOrEmpty(s.Direction))
            .GroupBy(s => s.Direction)
            .Select(g => new
            {
                DirectionName = g.Key,
                Specialties = g.Select(s => new
                {
                    Id = s.Id,
                    SpecialtyName = s.Name,
                    Qualification = s.Qualification
                }).OrderBy(s => s.SpecialtyName).ToList()
            })
            .OrderBy(g => g.DirectionName)
            .ToList();

        return View(groupedByDirection);
    }
}