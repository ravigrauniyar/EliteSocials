using EliteSocials.Controllers;
using EliteSocials.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use an in-memory cache for storing session data
builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "EliteSocialsCookies";
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<BaseController>();
builder.Services.AddScoped<SessionHandler<UserViewModel>>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EliteSocials}/{action=Index}/{id?}"
);

app.Run();