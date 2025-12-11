using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Claims;


namespace MyWebApp.Controllers
{
    public class AcademicProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.AcademicPrograms
                .Include(ap => ap.Specialty)
                    .ThenInclude(s => s.Department)
                .Include(ap => ap.Discipline)
                    .ThenInclude(d => d.DisciplineTeachers)
                        .ThenInclude(dt => dt.Teacher)
                .Include(ap => ap.WorkLoads)
                    .ThenInclude(wl => wl.Sections)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (program == null)
                return NotFound();


            bool isAdmin = User.IsInRole("admin");


            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = 0;
            bool isUserIdValid = int.TryParse(currentUserIdStr, out currentUserId);


            bool isAssignedTeacher = false;
            if (isUserIdValid && program.Discipline != null)
            {
                isAssignedTeacher = program.Discipline.DisciplineTeachers
                    .Any(dt => dt.TeacherId == currentUserId);
            }


            bool isHeadOfDepartment = false;
            if (isUserIdValid && program.Specialty?.Department != null)
            {

                isHeadOfDepartment = (program.Specialty.Department.HeadId == currentUserId);
            }


            ViewBag.CanEdit = isAdmin || isAssignedTeacher;
            ViewBag.IsHeadOfDepartment = isHeadOfDepartment;
            ViewBag.IsAdmin = isAdmin;
            return View(program);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var program = await _context.AcademicPrograms
                .Include(ap => ap.Specialty)
                .ThenInclude(s => s.Department)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (program == null) return NotFound();


            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId)) return Forbid();


            bool isHeadOfDepartment = program.Specialty?.Department?.HeadId == currentUserId;

            if (!isHeadOfDepartment) return Forbid();



            if (program.Status == "draft" || program.Status == "recheck")
            {
                if (newStatus == "approved" || newStatus == "rejected")
                {
                    program.Status = newStatus;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Details), new { id = program.Id });
        }


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View(new AcademicProgram());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(
            [Bind("Name,SpecialtyId,DisciplineId,StartYear,Goals,Requirements,DisciplinePosition,Literature,Status,WorkLoads,Competencies")] AcademicProgram program)
        {
            ApplyDefaultStatusAndClearModelErrors(program);

            if (ModelState.IsValid)
            {
                var specialty = await _context.Specialties.FirstOrDefaultAsync(s => s.Id == program.SpecialtyId);

                if (specialty == null)
                {
                    ModelState.AddModelError("SpecialtyId", "Специальность не найдена.");
                }
                else
                {
                    _context.Add(program);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            await PopulateDropDowns(program.SpecialtyId, program.DisciplineId);
            return View(program);
        }

        [HttpGet]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();


            var program = await _context.AcademicPrograms
                .Include(ap => ap.WorkLoads)
                    .ThenInclude(wl => wl.Sections)
                .Include(ap => ap.Discipline)
                    .ThenInclude(d => d.DisciplineTeachers)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (program == null) return NotFound();


            bool isAdmin = User.IsInRole("admin");


            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = 0;
            bool isUserIdValid = int.TryParse(currentUserIdStr, out currentUserId);


            bool isAssignedTeacher = false;
            if (isUserIdValid && program.Discipline != null)
            {
                isAssignedTeacher = program.Discipline.DisciplineTeachers
                    .Any(dt => dt.TeacherId == currentUserId);
            }


            if (!isAdmin && !isAssignedTeacher)
            {
                return Forbid();
            }


            await PopulateDropDowns(program.SpecialtyId, program.DisciplineId);

            return View(program);
        }

        [HttpPost]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SpecialtyId,DisciplineId,StartYear,Goals,Requirements,DisciplinePosition,Literature,Status,WorkLoads,Competencies")] AcademicProgram program)
        {
            if (id != program.Id) return NotFound();


            bool isAdmin = User.IsInRole("admin");
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = 0;
            bool isUserIdValid = int.TryParse(currentUserIdStr, out currentUserId);


            var programForAuth = await _context.AcademicPrograms
                .AsNoTracking()
                .Include(ap => ap.Discipline)
                    .ThenInclude(d => d.DisciplineTeachers)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (programForAuth == null) return NotFound();

            bool isAssignedTeacher = false;
            if (isUserIdValid && programForAuth.Discipline != null)
            {
                isAssignedTeacher = programForAuth.Discipline.DisciplineTeachers
                    .Any(dt => dt.TeacherId == currentUserId);
            }


            if (!isAdmin && !isAssignedTeacher)
            {
                return Forbid();
            }



            var originalStatus = await _context.AcademicPrograms
                .Where(p => p.Id == id)
                .Select(p => p.Status)
                .FirstOrDefaultAsync();

            ApplyDefaultStatusAndClearModelErrors(program);

            if ((originalStatus?.ToLower() == "approved" || originalStatus?.ToLower() == "rejected") && program.Status?.ToLower() != "recheck")
            {

                program.Status = "recheck";
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await UpdateNestedCollections(program);

                    var trackedProgram = _context.ChangeTracker.Entries<AcademicProgram>()
                        .FirstOrDefault(e => e.Entity.Id == program.Id)?.Entity;

                    if (trackedProgram != null)
                    {
                        _context.Entry(trackedProgram).CurrentValues.SetValues(program);
                    }
                    else
                    {
                        _context.Update(program);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ошибка при сохранении данных: " + ex.Message);
                    await PopulateDropDowns(program.SpecialtyId, program.DisciplineId);
                    return View(program);
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns(program.SpecialtyId, program.DisciplineId);
            return View(program);
        }

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
                competencies = specialty.Qualification ?? "",
            });
        }

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

        private async Task PopulateDropDowns(int? specialtyId = null, int? disciplineId = null)
        {
            ViewBag.Specialties = new SelectList(
                await _context.Specialties.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", specialtyId);
            ViewBag.Disciplines = new SelectList(
                await _context.Disciplines.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", disciplineId);
        }
        private bool AcademicProgramExists(int id)
        {
            return _context.AcademicPrograms.Any(e => e.Id == id);
        }

        private void ApplyDefaultStatusAndClearModelErrors(AcademicProgram program)
        {
            if (string.IsNullOrEmpty(program.Status)) program.Status = "draft";


            ModelState.Remove("Specialty");
            ModelState.Remove("Discipline");
            ModelState.Remove("Competencies");


            if (program.WorkLoads == null || !program.WorkLoads.Any())
            {
                ModelState.Remove("WorkLoads");
            }
            else
            {
                for (int i = 0; i < program.WorkLoads.Count; i++)
                {
                    ModelState.Remove($"WorkLoads[{i}].AcademicProgram");
                    var workload = program.WorkLoads.ElementAt(i);
                    if (workload.Sections != null)
                    {
                        for (int j = 0; j < workload.Sections.Count; j++)
                        {
                            ModelState.Remove($"WorkLoads[{i}].Sections[{j}].WorkLoad");
                        }
                    }
                }
            }

            var keysWithErrors = ModelState.Keys.Where(k => ModelState[k].Errors.Count > 0).ToList();

            foreach (var key in keysWithErrors)
            {
                var errors = ModelState[key].Errors;
                var invalidValueErrors = errors
                    .Where(e => e.ErrorMessage.Contains("is invalid") || e.ErrorMessage.Contains("недействительно") || e.ErrorMessage.Contains("The value ''"))
                    .ToList();

                if (invalidValueErrors.Any())
                {
                    var attempt = ModelState[key].AttemptedValue;
                    if (string.IsNullOrEmpty(attempt))
                    {
                        ModelState.Remove(key);
                    }
                }
            }
        }


        private async Task UpdateNestedCollections(AcademicProgram program)
        {

            var existingProgram = await _context.AcademicPrograms
                .Include(ap => ap.WorkLoads)
                    .ThenInclude(wl => wl.Sections)
                .AsNoTracking()
                .FirstOrDefaultAsync(ap => ap.Id == program.Id);

            if (existingProgram == null) return;


            var incomingWorkLoadIds = program.WorkLoads?.Where(wl => wl.Id != 0).Select(wl => wl.Id).ToList() ?? new List<int>();

            var workLoadsToDelete = existingProgram.WorkLoads
                .Where(wl => !incomingWorkLoadIds.Contains(wl.Id))
                .ToList();

            foreach (var workLoad in workLoadsToDelete)
            {

                _context.Sections.RemoveRange(workLoad.Sections);
                _context.WorkLoads.Remove(workLoad);
            }


            if (program.WorkLoads != null)
            {
                foreach (var incomingWorkLoad in program.WorkLoads)
                {
                    if (incomingWorkLoad.Id == 0)
                    {

                        incomingWorkLoad.AcademicProgramId = program.Id;
                        _context.WorkLoads.Add(incomingWorkLoad);
                    }
                    else
                    {

                        _context.WorkLoads.Update(incomingWorkLoad);


                        var existingWorkLoad = existingProgram.WorkLoads.FirstOrDefault(wl => wl.Id == incomingWorkLoad.Id);
                        if (existingWorkLoad != null)
                        {
                            var incomingSectionIds = incomingWorkLoad.Sections?.Where(s => s.Id != 0).Select(s => s.Id).ToList() ?? new List<int>();


                            var sectionsToDelete = existingWorkLoad.Sections
                                .Where(s => !incomingSectionIds.Contains(s.Id))
                                .ToList();
                            _context.Sections.RemoveRange(sectionsToDelete);


                            if (incomingWorkLoad.Sections != null)
                            {
                                foreach (var incomingSection in incomingWorkLoad.Sections)
                                {
                                    if (incomingSection.Id == 0)
                                    {
                                        incomingSection.WorkLoadId = incomingWorkLoad.Id;
                                        _context.Sections.Add(incomingSection);
                                    }
                                    else
                                    {
                                        _context.Sections.Update(incomingSection);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            await _context.SaveChangesAsync();


            program.WorkLoads = null;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var program = await _context.AcademicPrograms
                .Include(p => p.WorkLoads)
                    .ThenInclude(w => w.Sections)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program != null)
            {

                foreach (var workload in program.WorkLoads.ToList())
                {

                    _context.Sections.RemoveRange(workload.Sections);
                }


                _context.WorkLoads.RemoveRange(program.WorkLoads);


                _context.AcademicPrograms.Remove(program);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private static string GetStatusDisplayName(string status)
        {

            return status switch
            {
                "approved" => "Утверждена",
                "rejected" => "Отклонена",
                "recheck" => "На доработку",
                "draft" => "Черновик",
                _ => "Неизвестный статус"
            };
        }

        public async Task<IActionResult> Download(int id)
        {

            var program = await _context.AcademicPrograms
                .Include(ap => ap.Specialty)
                .Include(ap => ap.Discipline)
                    .ThenInclude(d => d.DisciplineTeachers)
                        .ThenInclude(dt => dt.Teacher)
                .Include(ap => ap.WorkLoads)
                    .ThenInclude(wl => wl.Sections)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (program == null) return NotFound();

            using var ms = new MemoryStream();


            Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            doc.Open();


            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            if (!System.IO.File.Exists(fontPath))
            {
                fontPath = @"C:\Windows\Fonts\arial.ttf";
            }

            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);


            Font fontTitle = new Font(baseFont, 16, Font.BOLD);
            Font fontSectionHeader = new Font(baseFont, 14, Font.BOLD);
            Font fontBold = new Font(baseFont, 10, Font.BOLD);
            Font fontNormal = new Font(baseFont, 10, Font.NORMAL);
            Font fontSmall = new Font(baseFont, 9, Font.NORMAL);


            var titleParagraph = new Paragraph($"Учебная программа: {program.Name}", fontTitle);
            titleParagraph.Alignment = Element.ALIGN_CENTER;
            titleParagraph.SpacingAfter = 20f;
            doc.Add(titleParagraph);


            PdfPTable mainInfoTable = new PdfPTable(4);
            mainInfoTable.WidthPercentage = 100;
            mainInfoTable.SetWidths(new float[] { 20, 30, 20, 30 });
            mainInfoTable.SpacingAfter = 20f;


            AddSimpleCell(mainInfoTable, "Название программы", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.Name, fontNormal, 1);
            AddSimpleCell(mainInfoTable, "Направление", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.Specialty?.Direction, fontNormal, 1);


            AddSimpleCell(mainInfoTable, "Дисциплина", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.Discipline?.Name, fontNormal, 1);
            AddSimpleCell(mainInfoTable, "Срок обучения", fontBold, 1);

            AddSimpleCell(mainInfoTable, program.Specialty?.Duration.ToString(), fontNormal, 1);


            AddSimpleCell(mainInfoTable, "Специальность", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.Specialty?.Name, fontNormal, 1);
            AddSimpleCell(mainInfoTable, "Квалификация", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.Specialty?.Qualification, fontNormal, 1);



            AddSimpleCell(mainInfoTable, "Год начала", fontBold, 1);
            AddSimpleCell(mainInfoTable, program.StartYear.ToString(), fontNormal, 1);

            AddSimpleCell(mainInfoTable, "Статус", fontBold, 1);
            string russianStatus = GetStatusDisplayName(program.Status);
            AddSimpleCell(mainInfoTable, russianStatus, fontNormal, 1);

            AddSimpleCell(mainInfoTable, "", fontNormal, 2);

            doc.Add(mainInfoTable);

            doc.Add(new Paragraph("Преподавательский состав", fontSectionHeader) { SpacingBefore = 10f, SpacingAfter = 10f });

            if (program.Discipline?.DisciplineTeachers != null && program.Discipline.DisciplineTeachers.Any())
            {
                PdfPTable teachersTable = new PdfPTable(2);
                teachersTable.WidthPercentage = 100;
                teachersTable.SetWidths(new float[] { 70, 30 });
                teachersTable.DefaultCell.BorderColor = BaseColor.BLACK;


                AddSimpleHeaderCell(teachersTable, "Преподаватель", fontBold);
                AddSimpleHeaderCell(teachersTable, "Тип участия", fontBold);

                foreach (var dt in program.Discipline.DisciplineTeachers)
                {
                    teachersTable.AddCell(new Phrase($"{dt.Teacher.Surname} {dt.Teacher.Name} {dt.Teacher.Patronymic}", fontNormal));
                    teachersTable.AddCell(new Phrase(dt.ParticipationType, fontNormal));
                }
                doc.Add(teachersTable);
            }
            else
            {
                doc.Add(new Paragraph("Преподаватели не назначены.", fontNormal));
            }


            doc.Add(new Paragraph("Учебный план по семестрам", fontSectionHeader) { SpacingBefore = 20f, SpacingAfter = 10f });

            if (program.WorkLoads != null && program.WorkLoads.Any())
            {
                foreach (var wl in program.WorkLoads.OrderBy(w => w.Semester))
                {

                    doc.Add(new Paragraph($"СЕМЕСТР {wl.Semester}", fontBold) { SpacingBefore = 10f, SpacingAfter = 5f });


                    PdfPTable statsTable = new PdfPTable(5);
                    statsTable.WidthPercentage = 100;
                    statsTable.DefaultCell.BorderColor = BaseColor.BLACK;

                    AddSimpleHeaderCell(statsTable, "Лекции", fontBold);
                    AddSimpleHeaderCell(statsTable, "Лаб.", fontBold);
                    AddSimpleHeaderCell(statsTable, "Практ. (СРС)", fontBold);
                    AddSimpleHeaderCell(statsTable, "Аттестация", fontBold);
                    AddSimpleHeaderCell(statsTable, "Общая СРС", fontBold);

                    statsTable.AddCell(new Phrase(wl.Lectures.ToString(), fontNormal));
                    statsTable.AddCell(new Phrase(wl.Labs.ToString(), fontNormal));
                    statsTable.AddCell(new Phrase(wl.SelfStudy.ToString(), fontNormal));
                    statsTable.AddCell(new Phrase(wl.IntermediateAssessment.ToString(), fontNormal));
                    statsTable.AddCell(new Phrase(wl.AssessmentType, fontNormal));

                    doc.Add(statsTable);


                    doc.Add(new Paragraph("Разделы дисциплины:", fontBold) { SpacingBefore = 10f });

                    if (wl.Sections != null && wl.Sections.Any())
                    {
                        PdfPTable secTable = new PdfPTable(7);
                        secTable.WidthPercentage = 100;
                        secTable.SetWidths(new float[] { 5, 20, 30, 10, 10, 10, 15 });
                        secTable.SpacingBefore = 5f;
                        secTable.DefaultCell.BorderColor = BaseColor.BLACK;

                        AddSimpleHeaderCell(secTable, "#", fontBold);
                        AddSimpleHeaderCell(secTable, "Название раздела", fontBold);
                        AddSimpleHeaderCell(secTable, "Краткое содержание", fontBold);
                        AddSimpleHeaderCell(secTable, "Лек.", fontBold);
                        AddSimpleHeaderCell(secTable, "Лаб.", fontBold);
                        AddSimpleHeaderCell(secTable, "Сем.", fontBold);
                        AddSimpleHeaderCell(secTable, "СРС", fontBold);

                        foreach (var s in wl.Sections.OrderBy(sec => sec.Index))
                        {
                            secTable.AddCell(new Phrase(s.Index.ToString(), fontSmall));
                            secTable.AddCell(new Phrase(s.Name, fontSmall));
                            secTable.AddCell(new Phrase(s.Description, fontSmall));
                            secTable.AddCell(new PdfPCell(new Phrase(s.LectureHours.ToString(), fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
                            secTable.AddCell(new PdfPCell(new Phrase(s.LabHours.ToString(), fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
                            secTable.AddCell(new PdfPCell(new Phrase(s.SeminarHours.ToString(), fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
                            secTable.AddCell(new PdfPCell(new Phrase(s.SelfStudyHours.ToString(), fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        }
                        doc.Add(secTable);
                    }
                    else
                    {
                        doc.Add(new Paragraph("Разделы отсутствуют", fontNormal));
                    }
                }
            }

            doc.Add(new Paragraph("Методические материалы", fontSectionHeader) { SpacingBefore = 20f, SpacingAfter = 15f });

            AddSimpleTextBlock(doc, "Цели и задачи", program.Goals, fontBold, fontNormal);
            AddSimpleTextBlock(doc, "Компетенции", program.Competencies, fontBold, fontNormal);
            AddSimpleTextBlock(doc, "Требования", program.Requirements, fontBold, fontNormal);
            AddSimpleTextBlock(doc, "Место дисциплины", program.DisciplinePosition, fontBold, fontNormal);
            AddSimpleTextBlock(doc, "Литература", program.Literature, fontBold, fontNormal);

            doc.Close();

            string fileName = $"Program_{program.Id}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }

        private void AddSimpleCell(PdfPTable table, string content, Font font, int colspan = 1)
        {
            var cell = new PdfPCell(new Phrase(content ?? "-", font));
            cell.Colspan = colspan;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.BLACK;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);
        }

        private void AddSimpleHeaderCell(PdfPTable table, string text, Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.BLACK;
            table.AddCell(cell);
        }

        private void AddSimpleTextBlock(Document doc, string title, string content, Font fontTitle, Font fontBody)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                doc.Add(new Paragraph(title, fontTitle) { SpacingBefore = 10f });

                var p = new Paragraph(content, fontBody);
                p.SpacingAfter = 10f;
                doc.Add(p);
            }
        }
    }
}