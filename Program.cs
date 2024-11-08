using Microsoft.EntityFrameworkCore;
using Movie2024.Models;

var builder = WebApplication.CreateBuilder(args);

//���o��Ʈw�s�u
builder.Services.AddDbContext<movieContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("movieContext")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// �t�m��� Cookie ���������ҡA�ó]�m�w�]���
builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.LoginPath = "/Helper/Index";
        options.AccessDeniedPath = "/Home/Index";
    });

// Anti-forgery token �]�w
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

// �]�m Cookie �{�Ҫ��ﶵ�A���w�n�J����
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ReturnUrlParameter = "returnUrl";
});

var app = builder.Build();

// �t�m HTTP �ШD�޹D
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // �ҥΨ�������
app.UseAuthorization();   // �ҥα��v


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
