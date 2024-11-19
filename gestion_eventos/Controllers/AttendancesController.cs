using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;
using gestion_eventos.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace gestion_eventos.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly GestionEventosContext _context;

        public AttendancesController(GestionEventosContext context)
        {
            _context = context;
        }

        // GET: Attendances/Index
        public async Task<IActionResult> Index()
        {
            var attendances = _context.Attendances
                .Include(a => a.Event)
                .Include(a => a.Person);
            return View(await attendances.ToListAsync());
        }

        // Método para descargar el reporte
        [HttpPost]
        public async Task<IActionResult> DownloadReport()
        {
            var events = await _context.Events
                .Include(e => e.Attendances)
                    .ThenInclude(a => a.Person)
                .Include(e => e.Snacks)
                .ToListAsync();

            var reportContent = new StringBuilder();
            foreach (var eventItem in events)
            {
                reportContent.AppendLine($"Evento: {eventItem.Title}");
                reportContent.AppendLine($"Descripción: {eventItem.Description}");
                reportContent.AppendLine($"Fecha: {eventItem.Date}");
                reportContent.AppendLine($"Ubicación: {eventItem.Location}");
                reportContent.AppendLine("Asistentes:");

                foreach (var attendance in eventItem.Attendances)
                {
                    reportContent.AppendLine($"- {attendance.Person.FullName} (Asistió: {(attendance.IsPresent ? "Sí" : "No")})");
                }

                reportContent.AppendLine("Snacks:");
                foreach (var snack in eventItem.Snacks)
                {
                    reportContent.AppendLine($"- {snack.SnackType} proporcionado el {snack.ProvidedOn}");
                }

                reportContent.AppendLine(new string('-', 40));
            }

            var reportBytes = Encoding.UTF8.GetBytes(reportContent.ToString());
            return File(reportBytes, "text/plain", "ReporteEventos.txt");
        }

        // Otros métodos en AttendancesController...

        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Event)
                .Include(a => a.Person)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // GET: Attendances/Create
        public IActionResult Create()
        {
            ViewBag.Events = _context.Events.Select(e => new { e.EventId, e.Title }).ToList();
            ViewBag.People = _context.People.Select(p => new { p.PersonId, p.FullName }).ToList();
            return View();
        }

        // POST: Attendances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceId,EventId,PersonId,IsPresent")] Attendance attendance)
        {
            if (true)
            {
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Events = _context.Events.Select(e => new { e.EventId, e.Title }).ToList();
            ViewBag.People = _context.People.Select(p => new { p.PersonId, p.FullName }).ToList();
            return View(attendance);
        }

        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            ViewBag.Events = _context.Events.Select(e => new { e.EventId, e.Title }).ToList();
            ViewBag.People = _context.People.Select(p => new { p.PersonId, p.FullName }).ToList();
            return View(attendance);
        }

        // POST: Attendances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceId,EventId,PersonId,IsPresent")] Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceId))
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
            ViewBag.Events = _context.Events.Select(e => new { e.EventId, e.Title }).ToList();
            ViewBag.People = _context.People.Select(p => new { p.PersonId, p.FullName }).ToList();
            return View(attendance);
        }

        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Event)
                .Include(a => a.Person)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}
