using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyWebApp.Controllers
{
    public class AcademicProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. СПИСОК (INDEX) ---
        public async Task<IActionResult> Index()
        {
            var programs = await _context.AcademicPrograms
                .Include(ap => ap.Specialty)
                .Include(ap => ap.Discipline)
                .OrderBy(ap => ap.StartYear)
                .ToListAsync();

            ViewBag.IsAdmin = User.IsInRole("admin");
            return View(programs);
        }

        // --- 2. СОЗДАНИЕ (CREATE GET) ---
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Specialties = new SelectList(
                await _context.Specialties.OrderBy(s => s.Name).ToListAsync(), "Id", "Name");
            ViewBag.Disciplines = new SelectList(
                await _context.Disciplines.OrderBy(d => d.Name).ToListAsync(), "Id", "Name");

            return View(new AcademicProgram());
        }

        // --- 3. AJAX ХЕЛПЕР (Для Specialty Details) ---
        [HttpGet]
        public async Task<IActionResult> GetSpecialtyDetails(int specialtyId)
        {
            var specialty = await _context.Specialties
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == specialtyId);

            if (specialty == null) return NotFound();

            return Json(new
            {
                direction = specialty.Direction,
                duration = specialty.Duration,
                competencies = specialty.Qualification ?? "Компетенции не указаны в Specialty",
            });
        }

        // --- 3b. AJAX ХЕЛПЕР (Для загрузки преподавателей по Дисциплине) ---
        [HttpGet]
        public async Task<IActionResult> GetDisciplineTeachers(int disciplineId)
        {
            var assignedTeachers = await _context.DisciplineTeachers
                .Where(dt => dt.DisciplineId == disciplineId)
                .Include(dt => dt.Teacher)
                .Select(dt => new
                {
                    id = dt.TeacherId,
                    name = $"{dt.Teacher.Surname} {dt.Teacher.Name} {dt.Teacher.Patronymic}",
                    participationType = dt.ParticipationType
                })
                .ToListAsync();

            var allTeachers = await _context.Users
                .Where(u => u.Role == "teacher")
                .OrderBy(u => u.Surname)
                .Select(u => new
                {
                    id = u.Id,
                    name = $"{u.Surname} {u.Name} {u.Patronymic}"
                })
                .ToListAsync();

            return Json(new
            {
                assignedTeachers = assignedTeachers,
                allTeachers = allTeachers
            });
        }

        // --- 4. СОЗДАНИЕ (CREATE POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(
            [Bind("Name,SpecialtyId,DisciplineId,StartYear,Goals,Requirements,DisciplinePosition,Literature,Status,WorkLoads")] AcademicProgram program)
        {
            // 1. Установка значения по умолчанию
            if (string.IsNullOrEmpty(program.Status)) program.Status = "draft";

            // 2. Очистка валидации для основных навигационных свойств
            ModelState.Remove("Specialty");
            ModelState.Remove("Discipline");
            ModelState.Remove("Competencies"); // Заполняется программно ниже

            // 3. Обработка списка WorkLoads (Вложенные коллекции)
            if (program.WorkLoads == null || !program.WorkLoads.Any())
            {
                // ИСПРАВЛЕНИЕ: "The WorkLoad field is required"
                // Если список пуст, просто удаляем ошибку требования списка
                ModelState.Remove("WorkLoads");
            }
            else
            {
                // Если список есть, нужно очистить циклические ссылки (Parent references)
                for (int i = 0; i < program.WorkLoads.Count; i++)
                {
                    // WorkLoad -> AcademicProgram
                    ModelState.Remove($"WorkLoads[{i}].AcademicProgram");

                    var workload = program.WorkLoads.ElementAt(i);
                    if (workload.Sections != null)
                    {
                        for (int j = 0; j < workload.Sections.Count; j++)
                        {
                            // Section -> WorkLoad
                            ModelState.Remove($"WorkLoads[{i}].Sections[{j}].WorkLoad");
                        }
                    }
                }
            }

            // =================================================================================
            // 4. ГЛОБАЛЬНОЕ ИСПРАВЛЕНИЕ ОШИБКИ: "The value '' is invalid"
            // =================================================================================
            // Мы проходим по всем ошибкам валидации. Если ошибка говорит о неверном значении (''),
            // и при этом пришло пустое значение или null, мы принудительно удаляем эту ошибку.
            // Это позволяет int полям оставаться 0 или null без блокировки сохранения.
            
            var keysWithErrors = ModelState.Keys
                .Where(k => ModelState[k].Errors.Count > 0)
                .ToList();

            foreach (var key in keysWithErrors)
            {
                var errors = ModelState[key].Errors;
                
                // Ищем ошибки конвертации типов (пустая строка в число)
                var invalidValueErrors = errors
                    .Where(e => e.ErrorMessage.Contains("is invalid") || 
                                e.ErrorMessage.Contains("недействительно") ||
                                e.ErrorMessage.Contains("The value ''"))
                    .ToList();

                if (invalidValueErrors.Any())
                {
                    // Получаем значение, которое вызвало ошибку
                    var attempt = ModelState[key].AttemptedValue;

                    // Если пытались передать пустоту, и это вызвало ошибку -> удаляем её
                    if (string.IsNullOrEmpty(attempt))
                    {
                        ModelState.Remove(key);
                    }
                }
            }
            // =================================================================================

            if (ModelState.IsValid)
            {
                // Поиск специальности для заполнения компетенций
                var specialty = await _context.Specialties.FirstOrDefaultAsync(s => s.Id == program.SpecialtyId);

                if (specialty == null)
                {
                    ModelState.AddModelError("SpecialtyId", "Специальность не найдена.");
                }
                else
                {
                    // Если компетенции не введены вручную, берем из специальности
                    if (string.IsNullOrEmpty(program.Competencies)) 
                    {
                        program.Competencies = specialty.Qualification;
                    }
                    
                    _context.Add(program);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Если все же есть ошибки, перезагружаем списки для выпадающих меню
            ViewBag.Specialties = new SelectList(await _context.Specialties.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", program.SpecialtyId);
            ViewBag.Disciplines = new SelectList(await _context.Disciplines.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", program.DisciplineId);
            
            // Логирование ошибок для отладки (опционально, можно смотреть в Output при отладке)
            foreach (var modelStateKey in ModelState.Keys)
            {
                var value = ModelState[modelStateKey];
                foreach (var error in value.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }

            return View(program);
        }
    }
}