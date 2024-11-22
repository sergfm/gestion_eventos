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

        // Métodos existentes (no modificados)...

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

        // ---------------- NUEVAS IMPLEMENTACIONES ----------------

        // Método para renderizar la vista parcial del carrito
        public IActionResult CargarCarritoParcial()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return PartialView("_CarritoPartial", new List<dynamic>());
            }

            var carrito = _context.ShoppingCart
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Event.Title,
                    c.TicketsQuantity,
                    c.PricePerTicket,
                    c.TotalPrice
                }).ToList();

            ViewData["CartItems"] = carrito;

            return PartialView("_CarritoPartial", carrito);
        }

        // Método para agregar un elemento al carrito
        [HttpPost]
        public IActionResult AgregarAlCarrito(int eventId, int cantidad = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var evento = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (evento == null || evento.AvailableTickets < cantidad)
            {
                return BadRequest("No hay suficientes boletos disponibles.");
            }

            var cartItem = _context.ShoppingCart.FirstOrDefault(c => c.EventId == eventId && c.UserId == userId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    EventId = eventId,
                    UserId = userId,
                    TicketsQuantity = cantidad,
                    PricePerTicket = evento.Price,
                    TotalPrice = cantidad * evento.Price
                };
                _context.ShoppingCart.Add(cartItem);
            }
            else
            {
                cartItem.TicketsQuantity += cantidad;
                cartItem.TotalPrice = cartItem.TicketsQuantity * evento.Price;
                _context.ShoppingCart.Update(cartItem);
            }

            _context.SaveChanges();

            // Llamar a la función para actualizar el contador del carrito
            ActualizarCarrito();

            return RedirectToAction("Carrito");
        }

        // Acción para mostrar la vista principal del carrito
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

        // Acción para procesar la compra directa de una entrada
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

            // Redirige a la vista "ComprarEntrada" donde se selecciona la cantidad de entradas
            var viewModel = new PurchaseTicketViewModel
            {
                EventId = evento.EventId,
                EventTitle = evento.Title,
                AvailableTickets = evento.AvailableTickets,
                Price = evento.Price
            };

            return View(viewModel);
        }

        // Acción para procesar la compra cuando el usuario hace clic en "Agregar al carrito"
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

            // Reduce los boletos disponibles
            evento.AvailableTickets -= model.TicketsToBuy;

            // Crea una nueva entrada en el carrito de compras
            var cartItem = new ShoppingCart
            {
                UserId = userId,
                EventId = evento.EventId,
                TicketsQuantity = model.TicketsToBuy,
                PricePerTicket = evento.Price,
                TotalPrice = evento.Price * model.TicketsToBuy
            };

            _context.ShoppingCart.Add(cartItem);
            _context.Events.Update(evento);
            _context.SaveChanges();

            // Redirige al carrito para completar la compra
            return RedirectToAction("Carrito");
        }

        // Acción para procesar el "Checkout" del carrito
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var carrito = _context.ShoppingCart.Where(c => c.UserId == userId).ToList();

            foreach (var item in carrito)
            {
                var evento = _context.Events.FirstOrDefault(e => e.EventId == item.EventId);
                if (evento != null && evento.AvailableTickets >= item.TicketsQuantity)
                {
                    // Se procesa la compra
                    _context.TicketPurchases.Add(new TicketPurchase
                    {
                        UserId = userId,
                        EventId = item.EventId,
                        TicketsBought = item.TicketsQuantity,
                        PurchaseDate = DateTime.Now
                    });

                    // Se actualizan los boletos disponibles
                    evento.AvailableTickets -= item.TicketsQuantity;
                    _context.Events.Update(evento);
                }

                // Elimina los artículos del carrito
                _context.ShoppingCart.Remove(item);
            }

            _context.SaveChanges();

            return RedirectToAction("MisEventos");
        }

        // Nueva acción para obtener el contador de ítems en el carrito
        public IActionResult ActualizarCarrito()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { itemCount = 0 });
            }

            // Contamos el número de eventos distintos en el carrito
            var itemCount = _context.ShoppingCart
                .Where(c => c.UserId == userId)
                .Select(c => c.EventId)
                .Distinct()
                .Count();

            return Json(new { itemCount });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
