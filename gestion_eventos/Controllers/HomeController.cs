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

            var cartItem = _context.ShoppingCart.FirstOrDefault(c => c.EventId == model.EventId && c.UserId == userId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    EventId = model.EventId,
                    UserId = userId,
                    TicketsQuantity = model.TicketsToBuy,
                    PricePerTicket = evento.Price,
                    TotalPrice = model.TicketsToBuy * evento.Price
                };
                _context.ShoppingCart.Add(cartItem);
            }
            else
            {
                cartItem.TicketsQuantity += model.TicketsToBuy;
                cartItem.TotalPrice = cartItem.TicketsQuantity * evento.Price;
                _context.ShoppingCart.Update(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Carrito");
        }

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
                    EventId = tp.EventId,
                    Title = tp.Event.Title,
                    Date = tp.Event.Date,
                    TicketsBought = tp.TicketsBought
                }).ToList();

            return View(misEventos);
        }

        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var checkoutViewModel = new CheckoutViewModel();

            return View(checkoutViewModel);
        }

        [HttpPost]
        public IActionResult ConfirmarCompra(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

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
                    _context.TicketPurchases.Add(new TicketPurchase
                    {
                        UserId = userId,
                        EventId = item.EventId,
                        TicketsBought = item.TicketsQuantity,
                        PurchaseDate = DateTime.Now
                    });

                    evento.AvailableTickets -= item.TicketsQuantity;
                    _context.Events.Update(evento);
                }

                _context.ShoppingCart.Remove(item);
            }

            _context.SaveChanges();

            return RedirectToAction("MisEventos");
        }

        [HttpGet]
        public JsonResult ObtenerCantidadCarrito()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Json(0);
            }

            var cantidad = _context.ShoppingCart
                .Where(c => c.UserId == userId)
                .Select(c => c.EventId)
                .Distinct()
                .Count();

            return Json(cantidad);
        }

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

        public IActionResult EliminarEvento(int eventId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
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

        [HttpGet]
        public JsonResult ObtenerEstadisticas()
        {
            var estadisticas = new
            {
                TotalEventos = _context.Events.Count(),
                EntradasVendidas = _context.TicketPurchases.Sum(tp => tp.TicketsBought),
                UsuariosRegistrados = _context.People.Count(),
                ProximosEventos = _context.Events.Count(e => e.Date > DateTime.Now)
            };

            return Json(estadisticas);
        }
    }
}
