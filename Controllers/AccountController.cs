using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MyWebApp.Data; // Замени на имя твоего проекта
using MyWebApp.Models; // Замени на имя твоего проекта
using System.Linq; // Добавлено для FirstOrDefault

namespace MyWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Если пользователь уже вошёл, перенаправляем на главное меню
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                // 🚨 ИСПОЛЬЗУЕМ TEMP DATA ДЛЯ ОТОБРАЖЕНИЯ ОШИБКИ В ПРЕДСТАВЛЕНИИ
                TempData["ErrorMessage"] = "Логин и пароль обязательны.";
                return View();
            }

            // Проверка в БД (в реальном приложении ХРАНИ ХЭШ ПАРОЛЯ!)
            // ИСПОЛЬЗУЙТЕ .Where и .FirstOrDefault() для получения данных
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                // Создаём ClaimsIdentity с информацией о пользователе
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user.Surname} {user.Name}"),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role), // Устанавливаем роль
                    // Можно добавить и другие данные, например, ФИО
                    new Claim("FullName", $"{user.Surname} {user.Name} {user.Patronymic ?? ""}".Trim())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                // Создаём и записываем cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // После успешной авторизации - редирект на главное меню
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 🚨 ИСПОЛЬЗУЕМ TEMP DATA ДЛЯ ОТОБРАЖЕНИЯ ОШИБКИ В ПРЕДСТАВЛЕНИИ
                TempData["ErrorMessage"] = "Неверный логин или пароль.";
            }

            // Возвращаем представление с ошибкой (которая теперь передана через TempData)
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}