using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using gestion_eventos.Data;
using gestion_eventos.Models;
using QuestPDF.Helpers;

namespace gestion_eventos.Controllers
{
    public class TicketsController : Controller
    {
        private readonly GestionEventosContext _context;

        public TicketsController(GestionEventosContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets.Include(t => t.Event).Include(t => t.Person);
            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.Person)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title");
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "FullName");
            return View(new Ticket());
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,PurchaseDate,PersonId,EventId")] Ticket ticket)
        {
            if (true)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticket.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "FullName", ticket.PersonId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticket.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "FullName", ticket.PersonId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,PurchaseDate,PersonId,EventId")] Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticket.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "FullName", ticket.PersonId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.Person)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }

        // NUEVA ACCIÓN: Exportar lista de tickets a PDF con diseño mejorado
        public IActionResult ExportarPDF()
        {
            var tickets = _context.Tickets.Include(t => t.Event).Include(t => t.Person).ToList();

            using (var stream = new MemoryStream())
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.Header().AlignCenter().Text("Lista de Entradas").Bold().FontSize(20);

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).BorderColor(Colors.Black).AlignCenter().Padding(5).Text("ID").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Black).AlignCenter().Padding(5).Text("Evento").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Black).AlignCenter().Padding(5).Text("Persona").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Black).AlignCenter().Padding(5).Text("Fecha de Compra").Bold();
                            });

                            foreach (var ticket in tickets)
                            {
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(5).Text(ticket.TicketId.ToString());
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(5).Text(ticket.Event.Title);
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(5).Text(ticket.Person.FullName);
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(5).Text(ticket.PurchaseDate.ToShortDateString());
                            }
                        });

                        page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy}");
                    });
                });

                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "ListaDeTickets.pdf");
            }
        }
    }
}
