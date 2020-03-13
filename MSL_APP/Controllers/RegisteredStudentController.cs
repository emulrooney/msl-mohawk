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
    public class RegisteredStudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisteredStudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RegisteredStudent
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RegisteredStudent.Include(r => r.EligibleStudent).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RegisteredStudent/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredStudent = await _context.RegisteredStudent
                .Include(r => r.EligibleStudent)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registeredStudent == null)
            {
                return NotFound();
            }

            return View(registeredStudent);
        }

        // GET: RegisteredStudent/Create
        public IActionResult Create()
        {
            ViewData["EligibleId"] = new SelectList(_context.EligibleStudent, "Id", "StudentEmail");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: RegisteredStudent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,EligibleId,StudentId,FirstName,LastName,StudentEmail")] RegisteredStudent registeredStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registeredStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EligibleId"] = new SelectList(_context.EligibleStudent, "Id", "StudentEmail", registeredStudent.EligibleId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", registeredStudent.UserId);
            return View(registeredStudent);
        }

        // GET: RegisteredStudent/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredStudent = await _context.RegisteredStudent.FindAsync(id);
            if (registeredStudent == null)
            {
                return NotFound();
            }
            ViewData["EligibleId"] = new SelectList(_context.EligibleStudent, "Id", "StudentEmail", registeredStudent.EligibleId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", registeredStudent.UserId);
            return View(registeredStudent);
        }

        // POST: RegisteredStudent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,EligibleId,StudentId,FirstName,LastName,StudentEmail")] RegisteredStudent registeredStudent)
        {
            if (id != registeredStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registeredStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisteredStudentExists(registeredStudent.Id))
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
            ViewData["EligibleId"] = new SelectList(_context.EligibleStudent, "Id", "StudentEmail", registeredStudent.EligibleId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", registeredStudent.UserId);
            return View(registeredStudent);
        }

        // GET: RegisteredStudent/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredStudent = await _context.RegisteredStudent
                .Include(r => r.EligibleStudent)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registeredStudent == null)
            {
                return NotFound();
            }

            return View(registeredStudent);
        }

        // POST: RegisteredStudent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registeredStudent = await _context.RegisteredStudent.FindAsync(id);
            _context.RegisteredStudent.Remove(registeredStudent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisteredStudentExists(int id)
        {
            return _context.RegisteredStudent.Any(e => e.Id == id);
        }
    }
}
