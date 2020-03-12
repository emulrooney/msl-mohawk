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
    public class ProductKeyLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductKeyLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductKeyLog
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductKeyLog.ToListAsync());
        }

        // GET: ProductKeyLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKeyLog = await _context.ProductKeyLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productKeyLog == null)
            {
                return NotFound();
            }

            return View(productKeyLog);
        }

        // GET: ProductKeyLog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductKeyLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,StudentId,StudentEmail,Action,ProductName,ProductKey,TimeStamp")] ProductKeyLog productKeyLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productKeyLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productKeyLog);
        }

        // GET: ProductKeyLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKeyLog = await _context.ProductKeyLog.FindAsync(id);
            if (productKeyLog == null)
            {
                return NotFound();
            }
            return View(productKeyLog);
        }

        // POST: ProductKeyLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,StudentId,StudentEmail,Action,ProductName,ProductKey,TimeStamp")] ProductKeyLog productKeyLog)
        {
            if (id != productKeyLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productKeyLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductKeyLogExists(productKeyLog.Id))
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
            return View(productKeyLog);
        }

        // GET: ProductKeyLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKeyLog = await _context.ProductKeyLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productKeyLog == null)
            {
                return NotFound();
            }

            return View(productKeyLog);
        }

        // POST: ProductKeyLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productKeyLog = await _context.ProductKeyLog.FindAsync(id);
            _context.ProductKeyLog.Remove(productKeyLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductKeyLogExists(int id)
        {
            return _context.ProductKeyLog.Any(e => e.Id == id);
        }
    }
}
