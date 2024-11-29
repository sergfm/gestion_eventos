using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;
using gestion_eventos.Models;
using ClosedXML.Excel;
using System.IO;
using System.Net.Http;
using ClosedXML.Excel.Drawings;

namespace gestion_eventos.Controllers
{
    public class EventsController : Controller
    {
        private readonly GestionEventosContext _context;

        public EventsController(GestionEventosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        public IActionResult Create()
        {
            return View(new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Title,Description,Date,Location,AvailableTickets,ImagenURL")] Event eventItem)
        {
            if (!string.IsNullOrEmpty(eventItem.ImagenURL))
            {
                string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                string fileName = $"{Guid.NewGuid()}.jpeg";
                string filePath = Path.Combine(imagesFolderPath, fileName);

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(eventItem.ImagenURL);
                    if (response.IsSuccessStatusCode)
                    {
                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                        eventItem.ImagenURL = $"/imagenes/{fileName}";
                    }
                }
            }

            if (true)
            {
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,Date,Location,AvailableTickets,ImagenURL")] Event eventItem)
        {
            if (id != eventItem.EventId)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(eventItem.ImagenURL))
            {
                string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                string fileName = $"{Guid.NewGuid()}.jpeg";
                string filePath = Path.Combine(imagesFolderPath, fileName);

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(eventItem.ImagenURL);
                    if (response.IsSuccessStatusCode)
                    {
                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                        eventItem.ImagenURL = $"/imagenes/{fileName}";
                    }
                }
            }

            if (true)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.EventId))
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
            return View(eventItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }

        // Exportar eventos a Excel con imágenes
        public IActionResult ExportarExcel()
        {
            var eventos = _context.Events.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Eventos");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Título";
                worksheet.Cell(currentRow, 3).Value = "Descripción";
                worksheet.Cell(currentRow, 4).Value = "Fecha";
                worksheet.Cell(currentRow, 5).Value = "Ubicación";
                worksheet.Cell(currentRow, 6).Value = "Entradas Disponibles";
                worksheet.Cell(currentRow, 7).Value = "Imagen";

                worksheet.Column(7).Width = 25;

                foreach (var evento in eventos)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = evento.EventId;
                    worksheet.Cell(currentRow, 2).Value = evento.Title;
                    worksheet.Cell(currentRow, 3).Value = evento.Description;
                    worksheet.Cell(currentRow, 4).Value = evento.Date.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(currentRow, 5).Value = evento.Location;
                    worksheet.Cell(currentRow, 6).Value = evento.AvailableTickets;

                    if (!string.IsNullOrEmpty(evento.ImagenURL))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", evento.ImagenURL.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            var cell = worksheet.Cell(currentRow, 7);
                            var picture = worksheet.AddPicture(imagePath)
                                .MoveTo(cell)
                                .Scale(0.02);
                        }
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListaDeEventos.xlsx");
                }
            }
        }
    }
}
