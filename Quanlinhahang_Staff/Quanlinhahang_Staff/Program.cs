using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Staff.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Session để lưu UserId Staff khi redirect từ Admin
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Kết nối DB Staff (cùng DB với Admin)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

// Bật Session
app.UseSession();

app.UseAuthorization();

// Route mặc định của Staff → vào Invoices
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Invoices}/{action=Index}/{id?}"
);

app.Run();
