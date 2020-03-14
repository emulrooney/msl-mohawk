using System;
using System.Collections.Generic;
using System.IO;
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
    [Authorize]
    public class ProductNameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductNameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductName Student View
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> IndexStudent()
        {
            return View(await _context.ProductName.ToListAsync());
        }

        // GET: ProductName Admin View
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductName.ToListAsync());
        }

        // GET: ProductName/Details/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,QuantityLimit,KeyCount,ActiveStatus,DownloadLink")] ProductName productName)
        {
            //Check if given name is a duplicate
            if (_context.ProductName.Any(p => p.Name == productName.Name))
            {
                throw new Exception($"Product '{productName.Name}' already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(productName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productName);
        }

        // GET: ProductName/Edit/5
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,QuantityLimit,KeyCount,ActiveStatus,DownloadLink")] ProductName productName)
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
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Upload file for Product Names. Creates a parser and uses it to parse the 
        /// user's file. Duplicate items shouldn't be uploaded.
        /// </summary>
        /// <param name="file">File data from file upload widget.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var parser = new CsvParser(file, ';');
            var results = parser.ParseProducts();

            foreach (string p in results.ValidList)
            {
                try
                {
                    await Create(new ProductName
                    {
                        Name = p,
                        QuantityLimit = 1
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

            return RedirectToAction("Index");
        }


    }
}
