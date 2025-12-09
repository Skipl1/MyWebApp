using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyWebApp.Data;
using MyWebApp.Models; 
using Microsoft.AspNetCore.Mvc.Rendering; // Обязательно для SelectListItem!
using iTextSharp.text;
using iTextSharp.text.pdf;

public class CurriculumController : Controller
{
    private readonly ApplicationDbContext _context;

    public CurriculumController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // ==========================================================
    // 1. ДЕЙСТВИЯ: ПРОСМОТР И ЗАГРУЗКА ДАННЫХ ДЛЯ ФОРМЫ (GET)
    // ==========================================================

    // GET /Curriculum/Index/{specialtyId}
    public async Task<IActionResult> Index(int specialtyId)
    {
        var specialty = await _context.Specialties
            .Include(s => s.Curricula)
                .ThenInclude(c => c.Discipline) 
            .Include(s => s.AcademicPrograms)
            .FirstOrDefaultAsync(s => s.Id == specialtyId);

        if (specialty == null)
        {
            return NotFound($"Специальность с ID={specialtyId} не найдена.");
        }
        
        // Безопасное получение списка учебных планов
        var safeCurricula = (specialty.Curricula as IEnumerable<Curriculum>) 
                            ?? Enumerable.Empty<Curriculum>(); 

        // Группировка данных для таблицы
        var disciplinesBySemester = safeCurricula
            .Where(c => c != null)
            .GroupBy(c => c.Semester)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Semester = g.Key,
                Disciplines = g.Select(c => new
                {
                    CurriculumId = c.Id, 
                    Name = c.Discipline?.Name ?? "Неизвестная дисциплина", 
                    CertificationType = c.CertificationType
                }).ToList()
            })
            .ToList();

        // --- ЛОГИКА ЗАГРУЗКИ ДАННЫХ ДЛЯ ФОРМЫ ДОБАВЛЕНИЯ (ViewData) ---

        // 1. Загружаем список всех дисциплин
        ViewData["AllDisciplines"] = await _context.Disciplines 
            .Select(d => new SelectListItem 
            {
                Value = d.Id.ToString(), 
                Text = d.Name
            })
            .ToListAsync();

        // 2. Загружаем список типов аттестации
        ViewData["AllCertificationTypes"] = new List<SelectListItem>
        {
            new SelectListItem { Value = "Экзамен", Text = "Экзамен" },
            new SelectListItem { Value = "Зачет", Text = "Зачет" },
            new SelectListItem { Value = "Курсовая работа", Text = "Курсовая работа" }
        };
        
        // 3. Передаем ID специальности
        ViewData["SpecialtyId"] = specialtyId;
        
        // Формирование итоговой модели
        var model = new
        {
            Specialty = specialty,
            PdfFilePath = specialty.AcademicPrograms?.FirstOrDefault()?.Goals,
            DisciplinesBySemester = disciplinesBySemester
        };

        return View(model);
    }
    
    // ==========================================================
    // 2. ДЕЙСТВИЯ: ДОБАВЛЕНИЕ (CRUD, только Admin)
    // ==========================================================

    // POST /Curriculum/Add
    [Authorize(Roles = "admin")] 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int SpecialtyId, int DisciplineId, int Semester, string CertificationType)
    {
        if (SpecialtyId <= 0 || DisciplineId <= 0 || Semester < 1 || Semester > 12 || string.IsNullOrEmpty(CertificationType))
        {
             // В случае ошибки возвращаемся на страницу, чтобы предотвратить сбой
             return RedirectToAction("Index", new { specialtyId = SpecialtyId });
        }
        
        var newCurriculumEntry = new Curriculum
        {
            SpecialtyId = SpecialtyId,
            DisciplineId = DisciplineId,
            Semester = Semester,
            CertificationType = CertificationType
        };

        _context.Curricula.Add(newCurriculumEntry);
        await _context.SaveChangesAsync();

        // Перенаправляем обратно на страницу деталей для обновления списка
        return RedirectToAction("Index", new { specialtyId = SpecialtyId });
    }
    
    // ==========================================================
    // 3. ДЕЙСТВИЯ: УДАЛЕНИЕ (CRUD, только Admin)
    // ==========================================================

    // POST /Curriculum/Delete
    [Authorize(Roles = "admin")] 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int curriculumId, int specialtyId)
    {
        var curriculumEntry = await _context.Curricula.FindAsync(curriculumId);
        
        if (curriculumEntry != null)
        {
            _context.Curricula.Remove(curriculumEntry);
            await _context.SaveChangesAsync();
        }
        
        // Перенаправляем обратно на страницу деталей
        return RedirectToAction("Index", new { specialtyId = specialtyId });
    }

    // ==========================================================
    // 4. ДЕЙСТВИЯ: СКАЧИВАНИЕ (Read, доступно всем)
    // ==========================================================

    // GET /Curriculum/Download/{specialtyId}
    public async Task<IActionResult> Download(int specialtyId)
    {
        var specialty = await _context.Specialties
            .Include(s => s.Curricula)
                .ThenInclude(c => c.Discipline)
            .FirstOrDefaultAsync(s => s.Id == specialtyId);

        if (specialty == null)
        {
            return NotFound("Специальность не найдена.");
        }

        var disciplines = (specialty.Curricula ?? new List<Curriculum>())
            .Where(c => c.Discipline != null)
            .OrderBy(c => c.Semester)
            .ToList();

        using (var ms = new MemoryStream())
        {
            // Создаем PDF документ
            var document = new Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Подключаем шрифт с поддержкой кириллицы
            string fontPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "arial.ttf"
            );

            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var fontTitle = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
            var fontHeader = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
            var fontText = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);

            // Заголовок
            document.Add(new Paragraph("Учебный план", fontTitle)
            {
                Alignment = Element.ALIGN_CENTER
            });

            document.Add(new Paragraph("\n"));

            // Информация о специальности
            document.Add(new Paragraph($"Специальность: {specialty.Name}", fontText));
            document.Add(new Paragraph($"Направление: {specialty.Direction}", fontText));
            document.Add(new Paragraph($"Продолжительность обучения: {specialty.Duration} лет", fontText));
            document.Add(new Paragraph($"Год утверждения: {DateTime.Now.Year}", fontText));
            document.Add(new Paragraph("\n"));

            // Заголовок таблицы
            document.Add(new Paragraph("Дисциплины:", fontHeader));
            document.Add(new Paragraph("\n"));

            // Таблица
            PdfPTable table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 60, 20, 20 });

            // Заголовки столбцов
            table.AddCell(new PdfPCell(new Phrase("Название дисциплины", fontHeader)));
            table.AddCell(new PdfPCell(new Phrase("Семестр", fontHeader)));
            table.AddCell(new PdfPCell(new Phrase("Тип аттестации", fontHeader)));

            // Данные
            foreach (var d in disciplines)
            {
                table.AddCell(new PdfPCell(new Phrase(d.Discipline.Name, fontText)));
                table.AddCell(new PdfPCell(new Phrase(d.Semester.ToString(), fontText)));
                table.AddCell(new PdfPCell(new Phrase(d.CertificationType, fontText)));
            }

            document.Add(table);

            document.Close();

            string fileName = $"Учебный_план_{specialty.Name}_{DateTime.Now.Year}.pdf";

            return File(ms.ToArray(), "application/pdf", fileName);
        }
    }
}