using System;
using System.Linq;
using System.Threading.Tasks;
using gestion_eventos.Data;
using gestion_eventos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestion_eventos.Controllers
{
    public class AuthController : Controller
    {
        private readonly GestionEventosContext _context;

        public AuthController(GestionEventosContext context)
        {
            _context = context;
        }

        // Página de inicio de sesión
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Buscar usuario por nombre de usuario y contraseña
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Guardar el rol en la sesión
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Role", user.Role);

                // Redirección según el rol del usuario
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home"); // Admin accede a todo
                }
                else if (user.Role == "Cliente")
                {
                    return RedirectToAction("Eventos", "Home"); // Cliente ve eventos disponibles
                }
                else if (user.Role == "Vendedor")
                {
                    return RedirectToAction("Ventas", "Home"); // Vendedor ve entradas vendidas
                }
            }

            // Mensaje de error si las credenciales no coinciden
            ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
            return View();
        }

        // Endpoint para inicio de sesión con API
        [HttpPost("api/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas." });
            }

            // Generar token de ejemplo (aquí podrías implementar un token JWT)
            var token = Guid.NewGuid().ToString();
            return Ok(new { token });
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        // Página de registro
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid) // Valida si el modelo es válido según las reglas definidas
            {
                // Validar que el correo electrónico no esté vacío o nulo
                if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Email", "El correo electrónico es obligatorio.");
                    return View(user); // Retorna la vista con el mensaje de error
                }

                // Agregar usuario a la base de datos
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Redirigir al inicio de sesión tras el registro exitoso
                return RedirectToAction("Login");
            }

            return View(user); // Retorna la vista si hay errores de validación
        }

        // Endpoint para registro con API
        [HttpPost("api/register")]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado." });
            }

            var user = new User
            {
                Username = request.UserName,
                Email = request.Email,
                Password = request.Password, // Nota: Considera encriptar la contraseña
                Role = "Cliente" // Asignar rol predeterminado
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente." });
        }
    }
}
