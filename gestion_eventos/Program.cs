using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar el servicio DbContext para utilizar SQL Server
builder.Services.AddDbContext<GestionEventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Habilitar servicios de sesión
builder.Services.AddSession();

// Habilitar controladores con vistas
builder.Services.AddControllersWithViews();

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Aplica la política de CORS antes de `UseAuthorization` y `UseSession`
app.UseCors("AllowAllOrigins");

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
