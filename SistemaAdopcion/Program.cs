using Microsoft.EntityFrameworkCore;
using SistemaAdopcion.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde las variables de entorno
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();

// Configurar ApplicationDbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // <- Importante para producción

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
