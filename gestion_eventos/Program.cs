using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;
using QuestPDF.Infrastructure; // Importar QuestPDF

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext para utilizar SQL Server
builder.Services.AddDbContext<GestionEventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar servicios de sesión con opciones específicas
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad
    options.Cookie.HttpOnly = true; // Seguridad adicional para cookies
    options.Cookie.IsEssential = true; // Necesario para el funcionamiento de la sesión
});

// Habilitar controladores con vistas y configurar validaciones
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; // Habilitar validación en cliente
    })
    .AddDataAnnotationsLocalization(); // Habilitar mensajes de validación localizados

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

// **Configuración de Autenticación JWT**
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// **Aceptar la licencia de QuestPDF**
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configuración para entornos de producción y desarrollo
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir a HTTPS
app.UseStaticFiles(); // Archivos estáticos (CSS, JS, etc.)

app.UseRouting();

// Aplica la política de CORS antes de las sesiones y autorizaciones
app.UseCors("AllowAllOrigins");

app.UseSession(); // Middleware de sesiones

// **Habilitar autenticación antes de autorización**
app.UseAuthentication();
app.UseAuthorization(); // Middleware de autorización

// Configurar las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
