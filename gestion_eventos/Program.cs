using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext para utilizar SQL Server
builder.Services.AddDbContext<GestionEventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar servicios de sesi�n con opciones espec�ficas
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad
    options.Cookie.HttpOnly = true; // Seguridad adicional para cookies
    options.Cookie.IsEssential = true; // Necesario para el funcionamiento de la sesi�n
});

// Habilitar controladores con vistas y configurar validaciones
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; // Habilitar validaci�n en cliente
    })
    .AddDataAnnotationsLocalization(); // Habilitar mensajes de validaci�n localizados

// Configurar CORS para permitir todas las solicitudes (si es necesario)
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

// Configuraci�n para entornos de producci�n y desarrollo
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir a HTTPS
app.UseStaticFiles(); // Archivos est�ticos (CSS, JS, etc.)

app.UseRouting();

// Aplica la pol�tica de CORS antes de las sesiones y autorizaciones
app.UseCors("AllowAllOrigins");

app.UseSession(); // Middleware de sesiones
app.UseAuthorization(); // Middleware de autorizaci�n

// Configurar las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
