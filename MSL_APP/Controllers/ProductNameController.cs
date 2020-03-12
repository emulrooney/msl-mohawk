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
    public class ProductNameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductNameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductName
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductName.ToListAsync());
        }

        // GET: ProductName/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productName = await _context.ProductName
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productName == null)
            {
                return NotFound();
            }

            return View(productName);
        }

        // GET: ProductName/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,QuantityLimit,KeyCount,ActiveStatus")] ProductName productName)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productName);
        }

        // GET: ProductName/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productName = await _context.ProductName.FindAsync(id);
            if (productName == null)
            {
                return NotFound();
            }
            return View(productName);
        }

        // POST: ProductName/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,QuantityLimit,KeyCount,ActiveStatus")] ProductName productName)
        {
            if (id != productName.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductNameExists(productName.Id))
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
            return View(productName);
        }

        // GET: ProductName/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productName = await _context.ProductName
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productName == null)
            {
                return NotFound();
            }

            return View(productName);
        }

        // POST: ProductName/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productName = await _context.ProductName.FindAsync(id);
            _context.ProductName.Remove(productName);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductNameExists(int id)
        {
            return _context.ProductName.Any(e => e.Id == id);
        }
    }
}
