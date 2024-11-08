using Microsoft.EntityFrameworkCore;
using Movie2024.Models;

var builder = WebApplication.CreateBuilder(args);

//取得資料庫連線
builder.Services.AddDbContext<movieContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("movieContext")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// 配置基於 Cookie 的身份驗證，並設置預設方案
builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.LoginPath = "/Helper/Index";
        options.AccessDeniedPath = "/Home/Index";
    });

// Anti-forgery token 設定
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

// 設置 Cookie 認證的選項，指定登入頁面
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ReturnUrlParameter = "returnUrl";
});

var app = builder.Build();

// 配置 HTTP 請求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // 啟用身份驗證
app.UseAuthorization();   // 啟用授權


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
