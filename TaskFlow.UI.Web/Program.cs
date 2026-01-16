using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using TaskFlow.IOC;

var builder = WebApplication.CreateBuilder(args);

// Registrar dependências
DependencyContainer.RegisterContainers(builder.Services);

// Configurar Dapper para mapear underscores automaticamente
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Configurar autenticação por Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Sair";
        options.AccessDeniedPath = "/Login/Index";
        options.Cookie.Name = ".TaskFlow";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(480); // 8 horas
    });

// Adicionar suporte a Controllers e Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar pipeline de requisição HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();