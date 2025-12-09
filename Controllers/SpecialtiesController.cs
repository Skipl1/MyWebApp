using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
// MyWebApp.ViewModels больше не нужен

public class SpecialtiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpecialtiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Specialties
    public async Task<IActionResult> Index()
    {
        // 1. Получаем все специальности
        var specialties = await _context.Specialties
            .ToListAsync();

        // 2. Группируем специальности по их Направлению (Direction) 
        // и создаем анонимный тип для передачи в представление
        var groupedByDirection = specialties
            .Where(s => !string.IsNullOrEmpty(s.Direction))
            .GroupBy(s => s.Direction)
            .Select(g => new // <-- Используем анонимный тип
            {
                DirectionName = g.Key, // Название направления
                Specialties = g.Select(s => new // <-- Вложенный анонимный тип для специальностей
                {
                    Id = s.Id,
                    SpecialtyName = s.Name
                }).OrderBy(s => s.SpecialtyName).ToList()
            })
            .OrderBy(g => g.DirectionName)
            .ToList(); // Тип: List<{ DirectionName: string, Specialties: List<{Id: int, SpecialtyName: string}> }>

        // Передаем список анонимных объектов в представление
        return View(groupedByDirection); 
    }
}