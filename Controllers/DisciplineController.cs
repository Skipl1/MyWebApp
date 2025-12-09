using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Microsoft.AspNetCore.Authorization; // Обязательно для атрибута [Authorize]
using System.Linq; 

namespace MyWebApp.Controllers
{
    [Authorize] 
    public class DisciplineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DisciplineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ДЕЙСТВИЯ, ДОСТУПНЫЕ ВСЕМ (ИЛИ ТОЛЬКО АУТЕНТИФИЦИРОВАННЫМ ПОЛЬЗОВАТЕЛЯМ) ---
        
        // GET: /Discipline/Index
        public async Task<IActionResult> Index(int? editId)
        {
            var disciplines = await _context.Disciplines
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.IsAdmin = User.IsInRole("admin");
            ViewBag.NewDiscipline = new Discipline();
            ViewBag.EditDisciplineId = editId; 
            
            return View(disciplines);
        }

        // --- ДЕЙСТВИЯ, ДОСТУПНЫЕ ТОЛЬКО АДМИНИСТРАТОРАМ ---
        
        // POST: /Discipline/Index (Добавление новой дисциплины)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> Index([Bind("Name")] Discipline newDiscipline)
        {
            // Здесь ModelState.IsValid проверяет модель Discipline, 
            // но мы также добавили явную проверку на пустую строку
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(newDiscipline.Name))
            {
                var discipline = new Discipline { Name = newDiscipline.Name };
                _context.Add(discipline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // В случае ошибки валидации (возвращаем View, чтобы сохранить ошибки в ModelState)
            var disciplines = await _context.Disciplines.OrderBy(d => d.Name).ToListAsync();
            ViewBag.IsAdmin = User.IsInRole("admin"); 
            ViewBag.NewDiscipline = newDiscipline; 
            
            return View(disciplines);
        }
        
        // POST: /Discipline/EditConfirmed/{id} (Сохранение изменений)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> EditConfirmed(int id, [Bind("Id,Name")] Discipline updatedDiscipline)
        {
            if (id != updatedDiscipline.Id) return NotFound(); 

            if (ModelState.IsValid)
            {
                // 1. Загружаем сущность из БД
                var disciplineToUpdate = await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);
                
                if (disciplineToUpdate == null) return NotFound();

                // 2. Применяем новое имя
                // (Для отладки: убедитесь, что updatedDiscipline.Name содержит новое значение)
                disciplineToUpdate.Name = updatedDiscipline.Name; 
                
                // 3. Сохраняем (Update вызывать не обязательно, EF сам увидит изменения)
                await _context.SaveChangesAsync();
                
                // 4. Перенаправляем на чистый Index
                return RedirectToAction(nameof(Index)); 
            }
            
            // В случае ошибки возвращаем View
            var disciplines = await _context.Disciplines.OrderBy(d => d.Name).ToListAsync();
            ViewBag.IsAdmin = User.IsInRole("admin");
            ViewBag.NewDiscipline = new Discipline();
            ViewBag.EditDisciplineId = id; 
            
            return View("Index", disciplines);
        }
    }
}