using gestion_eventos.Data;
using gestion_eventos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace gestion_eventos.Controllers
{
    public class HomeController : Controller
    {
        private readonly GestionEventosContext _context;

        public HomeController(GestionEventosContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Eventos");
            }

            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            ViewData["TotalEventos"] = _context.Events.Count();
            ViewData["EntradasVendidas"] = _context.TicketPurchases.Sum(tp => tp.TicketsBought);
            ViewData["UsuariosRegistrados"] = _context.People.Count();

            var asistencias = _context.TicketPurchases
                .Where(tp => tp.IsPresent)
                .Select(tp => new { tp.UserId, tp.Event.Title })
                .Distinct()
                .ToList();

            ViewData["PersonasAsistieron"] = asistencias.Count;

            var eventosEstadisticas = _context.Events
                .Select(e => new
                {
                    e.Title,
                    Vendidas = _context.TicketPurchases.Where(tp => tp.EventId == e.EventId).Sum(tp => tp.TicketsBought),
                    Disponibles = e.AvailableTickets
                }).ToList();

            var ventasPorMes = _context.TicketPurchases
                .GroupBy(tp => tp.PurchaseDate.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(tp => tp.TicketsBought) })
                .OrderBy(g => g.Mes)
                .ToList();

            ViewData["GraficoEventos"] = eventosEstadisticas;
            ViewData["GraficoMensual"] = ventasPorMes;
            ViewData["TablaAsistencias"] = asistencias;

            return View("Dashboard");
        }

        public IActionResult Eventos()
        {
            var eventos = _context.Events.ToList();
            return View(eventos);
        }

        public IActionResult Carrito()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var carrito = _context.ShoppingCart
                .Where(c => c.UserId == userId)
                .Select(c => new CarritoItemViewModel
                {
                    Title = c.Event.Title,
                    TicketsQuantity = c.TicketsQuantity,
                    PricePerTicket = c.PricePerTicket,
                    TotalPrice = c.TotalPrice,
                    CartId = c.CartId
                }).ToList();

            return View(carrito);
        }

        public IActionResult ComprarEntrada(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var evento = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (evento == null || evento.AvailableTickets <= 0)
            {
                return NotFound("Evento no disponible o entradas agotadas.");
            }

            var viewModel = new PurchaseTicketViewModel
            {
                EventId = evento.EventId,
                EventTitle = evento.Title,
                AvailableTickets = evento.AvailableTickets,
                Price = evento.Price
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ComprarEntrada(PurchaseTicketViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var evento = _context.Events.FirstOrDefault(e => e.EventId == model.EventId);
            if (evento == null || evento.AvailableTickets < model.TicketsToBuy)
            {
                return BadRequest("No hay suficientes boletos disponibles.");
            }

            evento.AvailableTickets -= model.TicketsToBuy;

            _context.TicketPurchases.Add(new TicketPurchase
            {
                EventId = model.EventId,
                UserId = userId,
                TicketsBought = model.TicketsToBuy,
                PurchaseDate = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("MisEventos");
        }

        // Acción corregida para listar los eventos comprados por el usuario
        public IActionResult MisEventos()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var misEventos = _context.TicketPurchases
                .Where(tp => tp.UserId == userId)
                .Select(tp => new MisEventosViewModel
                {
                    EventId = tp.EventId, // Corregido: ahora incluye el ID del evento
                    Title = tp.Event.Title,
                    Date = tp.Event.Date,
                    TicketsBought = tp.TicketsBought
                }).ToList();

            return View(misEventos);
        }
    }
}
