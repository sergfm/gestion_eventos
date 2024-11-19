using gestion_eventos.Data;
using gestion_eventos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace gestion_eventos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GestionEventosContext _context;

        public HomeController(ILogger<HomeController> logger, GestionEventosContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Implementación original no modificada
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Eventos");
            }

            return View();
        }

        public IActionResult Eventos()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Cliente" && role != "Admin")
            {
                return RedirectToAction("Index");
            }

            var eventos = _context.Events.ToList();

            return View(eventos);
        }

        public IActionResult Ventas()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Vendedor" && role != "Admin")
            {
                return RedirectToAction("Index");
            }

            ViewBag.Ventas = new List<(string Evento, int Entradas, decimal Total)>
            {
                ("Concierto A", 120, 1200.00M),
                ("Conferencia B", 80, 800.00M),
                ("Feria C", 150, 1500.00M)
            };

            return View("Vendedor");
        }

        public IActionResult Privacy()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Eventos");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ComprarEntrada(int id)
        {
            var evento = _context.Events.FirstOrDefault(e => e.EventId == id);

            if (evento == null || evento.AvailableTickets <= 0)
            {
                TempData["Error"] = "El evento no está disponible o no tiene boletos restantes.";
                return RedirectToAction("Eventos");
            }

            var model = new PurchaseTicketViewModel
            {
                EventId = evento.EventId,
                EventTitle = evento.Title,
                AvailableTickets = evento.AvailableTickets,
                Price = evento.Price // Incluye el precio del evento
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ComprarEntrada(PurchaseTicketViewModel model)
        {
            if (true)
            {
                var evento = _context.Events.FirstOrDefault(e => e.EventId == model.EventId);

                if (evento == null || evento.AvailableTickets < model.TicketsToBuy)
                {
                    ModelState.AddModelError("", "No hay suficientes boletos disponibles.");
                    return View(model);
                }

                evento.AvailableTickets -= model.TicketsToBuy;

                var userId = HttpContext.Session.GetString("UserId");
                var cartItem = new ShoppingCart
                {
                    EventId = evento.EventId,
                    UserId = userId,
                    TicketsQuantity = model.TicketsToBuy,
                    PricePerTicket = evento.Price,
                    TotalPrice = model.TicketsToBuy * evento.Price
                };

                _context.ShoppingCart.Add(cartItem);
                _context.SaveChanges();

                return RedirectToAction("Carrito");
            }

            return View(model);
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
                .Select(c => new
                {
                    c.Event.Title,
                    c.TicketsQuantity,
                    c.PricePerTicket,
                    c.TotalPrice,
                    c.CartId
                }).ToList();

            return View(carrito);
        }

        [HttpPost]
        public IActionResult EliminarDelCarrito(int cartId)
        {
            var cartItem = _context.ShoppingCart.FirstOrDefault(c => c.CartId == cartId);

            if (cartItem != null)
            {
                _context.ShoppingCart.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Carrito");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarCompra(CheckoutViewModel model)
        {
            if (true)
            {
                var userId = HttpContext.Session.GetString("UserId");

                var itemsCarrito = _context.ShoppingCart.Where(c => c.UserId == userId).ToList();

                foreach (var item in itemsCarrito)
                {
                    var purchase = new TicketPurchase
                    {
                        EventId = item.EventId,
                        UserId = item.UserId,
                        TicketsBought = item.TicketsQuantity,
                        PurchaseDate = DateTime.Now
                    };

                    _context.TicketPurchases.Add(purchase);
                }

                _context.ShoppingCart.RemoveRange(itemsCarrito);
                _context.SaveChanges();

                return RedirectToAction("MisEventos");
            }

            return View(model);
        }

        public IActionResult MisEventos()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var misCompras = _context.TicketPurchases
                .Where(tp => tp.UserId == userId)
                .Select(tp => new
                {
                    tp.Event.EventId,
                    tp.Event.Title,
                    tp.Event.Date,
                    tp.TicketsBought
                }).ToList();

            return View(misCompras);
        }

        [HttpPost]
        public IActionResult EliminarEvento(int eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth"); // Redirigir si no está autenticado
            }

            var ticketPurchase = _context.TicketPurchases
                .FirstOrDefault(tp => tp.EventId == eventId && tp.UserId == userId);

            if (ticketPurchase != null)
            {
                _context.TicketPurchases.Remove(ticketPurchase);
                _context.SaveChanges();
            }

            return RedirectToAction("MisEventos");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
