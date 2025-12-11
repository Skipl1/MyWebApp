using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index([Bind("Name")] Discipline newDiscipline)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(newDiscipline.Name))
            {
                var discipline = new Discipline { Name = newDiscipline.Name };
                _context.Add(discipline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            var disciplines = await _context.Disciplines.OrderBy(d => d.Name).ToListAsync();
            ViewBag.IsAdmin = User.IsInRole("admin");
            ViewBag.NewDiscipline = newDiscipline;

            return View(disciplines);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditConfirmed(int id, [Bind("Id,Name")] Discipline updatedDiscipline)
        {
            if (id != updatedDiscipline.Id) return NotFound();

            if (ModelState.IsValid)
            {

                var disciplineToUpdate = await _context.Disciplines.FirstOrDefaultAsync(d => d.Id == id);

                if (disciplineToUpdate == null) return NotFound();



                disciplineToUpdate.Name = updatedDiscipline.Name;


                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }

            var disciplines = await _context.Disciplines.OrderBy(d => d.Name).ToListAsync();
            ViewBag.IsAdmin = User.IsInRole("admin");
            ViewBag.NewDiscipline = new Discipline();
            ViewBag.EditDisciplineId = id;

            return View("Index", disciplines);
        }
    }
}