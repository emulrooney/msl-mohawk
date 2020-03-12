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
    public class ProductKeyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductKeyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductKey
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductKey.ToListAsync());
        }

        // GET: ProductKey/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKey = await _context.ProductKey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productKey == null)
            {
                return NotFound();
            }

            return View(productKey);
        }

        // GET: ProductKey/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductKey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameId,Key,ActiveStatus,OwnerId")] ProductKey productKey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productKey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productKey);
        }

        // GET: ProductKey/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKey = await _context.ProductKey.FindAsync(id);
            if (productKey == null)
            {
                return NotFound();
            }
            return View(productKey);
        }

        // POST: ProductKey/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameId,Key,ActiveStatus,OwnerId")] ProductKey productKey)
        {
            if (id != productKey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productKey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductKeyExists(productKey.Id))
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
            return View(productKey);
        }

        // GET: ProductKey/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKey = await _context.ProductKey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productKey == null)
            {
                return NotFound();
            }

            return View(productKey);
        }

        // POST: ProductKey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productKey = await _context.ProductKey.FindAsync(id);
            _context.ProductKey.Remove(productKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductKeyExists(int id)
        {
            return _context.ProductKey.Any(e => e.Id == id);
        }
    }
}
