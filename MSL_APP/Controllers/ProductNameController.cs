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
    [Authorize]
    public class ProductNameController : Controller
    {
        private readonly ApplicationDbContext _context;
        List<SelectListItem> activeStatus = new List<SelectListItem> 
        {
            new SelectListItem { Text = "Active", Value = "Actived" },
            new SelectListItem { Text = "Disable",  Value = "Disabled" },
        };
        public ProductNameController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: ProductName
        [Authorize(Roles = "Admin, Student")]
        public async Task<IActionResult> Student()
        {
            return View(await _context.ProductName.ToListAsync());
        }

        // GET: ProductName
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortBy, string search)
        {
            var products = _context.ProductName.AsQueryable();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            ViewBag.SortByProduct = string.IsNullOrEmpty(sortBy) ? "NameDESC" : "";
            ViewBag.SortByTotalKeys = sortBy == "TotalKey" ? "TotalKeyDESC" : "TotalKey";
            ViewBag.SortByLimit = sortBy == "QuantityLimit" ? "QuantityLimitDESC" : "QuantityLimit";
            ViewBag.SortByStatus = sortBy == "ActiveStatus" ? "ActiveStatusDESC" : "ActiveStatus";
            ViewBag.SortByLink = sortBy == "DownloadLink" ? "DownloadLinkDESC" : "DownloadLink";

            // Sort the product by name
            switch (sortBy)
            {
                case "NameDESC":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "TotalKeyDESC":
                    products = products.OrderByDescending(p => p.KeyCount);
                    break;
                case "TotalKey":
                    products = products.OrderBy(p => p.KeyCount);
                    break;
                case "QuantityLimitDESC":
                    products = products.OrderByDescending(p => p.QuantityLimit);
                    break;
                case "QuantityLimit":
                    products = products.OrderBy(p => p.QuantityLimit);
                    break;
                case "ActiveStatusDESC":
                    products = products.OrderByDescending(p => p.ActiveStatus);
                    break;
                case "ActiveStatus":
                    products = products.OrderBy(p => p.ActiveStatus);
                    break;
                case "DownloadLinkDESC":
                    products = products.OrderByDescending(p => p.DownloadLink);
                    break;
                case "DownloadLink":
                    products = products.OrderBy(p => p.DownloadLink);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return View(await products.ToListAsync());
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
            ViewBag.productstatus = activeStatus;
            return View();
        }

        // POST: ProductName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,QuantityLimit,KeyCount,UsedKeyCount,ActiveStatus,DownloadLink")] ProductName productName)
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
            ViewBag.productstatus = activeStatus;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,QuantityLimit,KeyCount,UsedKeyCount,ActiveStatus,DownloadLink")] ProductName productName)
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction("Index");

            var parser = new CsvParser(file, ';');
            var results = parser.ParseProducts();

            foreach (ProductName pn in results.ValidList)
            {
                ProductName existingRow = _context.ProductName.FirstOrDefault(p => p.Name == pn.Name);
                if (existingRow == null)
                    _context.ProductName.Add(pn);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
