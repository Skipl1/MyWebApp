using Microsoft.AspNetCore.Authentication.Cookies; // Добавь этот using
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data; // Замени 'MyWebApp' на имя твоего проекта

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// 🔑 Добавляем DbContext с PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// 🔐 Добавляем аутентификацию через Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Куда перенаправлять, если не авторизован
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Опционально
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Время жизни cookie
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();