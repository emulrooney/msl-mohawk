using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSL_APP.Data;
using MSL_APP.Models;

namespace MSL_APP.Controllers
{
    public class EligibleStudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EligibleStudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EligibleStudent
        public async Task<IActionResult> Index()
        {
            return View(await _context.EligibleStudent.ToListAsync());
        }

        // GET: EligibleStudent/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eligibleStudent = await _context.EligibleStudent
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eligibleStudent == null)
            {
                return NotFound();
            }

            return View(eligibleStudent);
        }

        // GET: EligibleStudent/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EligibleStudent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentID,FirstName,LastName,StudentEmail")] EligibleStudent eligibleStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eligibleStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eligibleStudent);
        }

        // GET: EligibleStudent/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eligibleStudent = await _context.EligibleStudent.FindAsync(id);
            if (eligibleStudent == null)
            {
                return NotFound();
            }
            return View(eligibleStudent);
        }

        // POST: EligibleStudent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentID,FirstName,LastName,StudentEmail")] EligibleStudent eligibleStudent)
        {
            if (id != eligibleStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eligibleStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EligibleStudentExists(eligibleStudent.Id))
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
            return View(eligibleStudent);
        }

        // GET: EligibleStudent/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eligibleStudent = await _context.EligibleStudent
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eligibleStudent == null)
            {
                return NotFound();
            }

            return View(eligibleStudent);
        }

        // POST: EligibleStudent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eligibleStudent = await _context.EligibleStudent.FindAsync(id);
            _context.EligibleStudent.Remove(eligibleStudent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EligibleStudentExists(int id)
        {
            return _context.EligibleStudent.Any(e => e.Id == id);
        }
    }
}
