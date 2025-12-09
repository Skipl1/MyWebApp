using Microsoft.AspNetCore.Authorization; // Добавь этот using
using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers
{
    [Authorize] // Требует авторизации для всех действий в этом контроллере
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Передаём в представление имя пользователя и его роль
            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return View();
        }
    }
}