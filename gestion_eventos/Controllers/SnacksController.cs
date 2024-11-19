using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestion_eventos.Data;
using gestion_eventos.Models;

namespace gestion_eventos.Controllers
{
    public class SnacksController : Controller
    {
        private readonly GestionEventosContext _context;

        public SnacksController(GestionEventosContext context)
        {
            _context = context;
        }

        // GET: Snacks
        public async Task<IActionResult> Index()
        {
            var gestionEventosContext = _context.Snacks.Include(s => s.Event).Include(s => s.Person);
            return View(await gestionEventosContext.ToListAsync());
        }

        // GET: Snacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snack = await _context.Snacks
                .Include(s => s.Event)
                .Include(s => s.Person)
                .FirstOrDefaultAsync(m => m.SnackId == id);
            if (snack == null)
            {
                return NotFound();
            }

            return View(snack);
        }

        // GET: Snacks/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId");
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId");
            return View();
        }

        // POST: Snacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SnackId,SnackType,ProvidedOn,PersonId,EventId")] Snack snack)
        {
            if (true)
            {
                _context.Add(snack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", snack.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", snack.PersonId);
            return View(snack);
        }

        // GET: Snacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", snack.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", snack.PersonId);
            return View(snack);
        }

        // POST: Snacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SnackId,SnackType,ProvidedOn,PersonId,EventId")] Snack snack)
        {
            if (id != snack.SnackId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(snack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SnackExists(snack.SnackId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", snack.EventId);
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", snack.PersonId);
            return View(snack);
        }

        // GET: Snacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snack = await _context.Snacks
                .Include(s => s.Event)
                .Include(s => s.Person)
                .FirstOrDefaultAsync(m => m.SnackId == id);
            if (snack == null)
            {
                return NotFound();
            }

            return View(snack);
        }

        // POST: Snacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var snack = await _context.Snacks.FindAsync(id);
            if (snack != null)
            {
                _context.Snacks.Remove(snack);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SnackExists(int id)
        {
            return _context.Snacks.Any(e => e.SnackId == id);
        }
    }
}
