using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzeriaCC.Models;

namespace PizzeriaCC.Controllers
{
    public class MotorizadoesController : Controller
    {
        private readonly PizzeriaBdContext _context;

        public MotorizadoesController(PizzeriaBdContext context)
        {
            _context = context;
        }

        // GET: Motorizadoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Motorizados.ToListAsync());
        }

        // GET: Motorizadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorizado = await _context.Motorizados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motorizado == null)
            {
                return NotFound();
            }

            return View(motorizado);
        }

        // GET: Motorizadoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Motorizadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Disponible,EntregasRealizadas")] Motorizado motorizado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(motorizado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(motorizado);
        }

        // GET: Motorizadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorizado = await _context.Motorizados.FindAsync(id);
            if (motorizado == null)
            {
                return NotFound();
            }
            return View(motorizado);
        }

        // POST: Motorizadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Disponible,EntregasRealizadas")] Motorizado motorizado)
        {
            if (id != motorizado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(motorizado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MotorizadoExists(motorizado.Id))
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
            return View(motorizado);
        }

        // GET: Motorizadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorizado = await _context.Motorizados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motorizado == null)
            {
                return NotFound();
            }

            return View(motorizado);
        }

        // POST: Motorizadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var motorizado = await _context.Motorizados.FindAsync(id);
            if (motorizado != null)
            {
                _context.Motorizados.Remove(motorizado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MotorizadoExists(int id)
        {
            return _context.Motorizados.Any(e => e.Id == id);
        }
    }
}
