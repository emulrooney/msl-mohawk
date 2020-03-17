using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSL_APP.Data;
using MSL_APP.Models;

namespace MSL_APP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductKeyController : Controller
    {
        private readonly ApplicationDbContext _context;
        List<SelectListItem> status = new List<SelectListItem>
        {
            new SelectListItem { Text = "New", Value = "New" },
            new SelectListItem { Text = "Used",  Value = "Used" },
        };
        public ProductKeyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductKey
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductKey.Include(p => p.ProductName);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductKey/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productKey = await _context.ProductKey
                .Include(p => p.ProductName)
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
            ViewBag.keystatus = status;
            ViewData["NameId"] = new SelectList(_context.ProductName, "Id", "Name");
            return View();
        }

        // POST: ProductKey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameId,Key,Status,OwnerId")] ProductKey productKey)
        {
            if (ModelState.IsValid)
            {
                // Increse current product key count by one

                _context.Add(productKey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NameId"] = new SelectList(_context.ProductName, "Id", "Name", productKey.NameId);
            return View(productKey);
        }

        // GET: ProductKey/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.keystatus = status;

            if (id == null)
            {
                return NotFound();
            }

            var productKey = await _context.ProductKey.FindAsync(id);
            if (productKey == null)
            {
                return NotFound();
            }
            ViewData["NameId"] = new SelectList(_context.ProductName, "Id", "Name", productKey.NameId);
            return View(productKey);
        }

        // POST: ProductKey/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameId,Key,Status,OwnerId")] ProductKey productKey)
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
            ViewData["NameId"] = new SelectList(_context.ProductName, "Id", "Name", productKey.NameId);
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
                .Include(p => p.ProductName)
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            return View("Error", new ErrorViewModel { RequestId = "Not yet implemented!" });
        }
    }
}
