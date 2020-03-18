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
using MSL_APP.Utility;

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
        public async Task<IActionResult> Index(string sortBy, string search, string currentFilter, int? pageNumber)
        {
            int pageSize = 10;
            ViewData["CurrentSort"] = sortBy;
            ViewData["KeyName"] = string.IsNullOrEmpty(sortBy) ? "KeyNameDESC" : "";
            ViewData["KeyCode"] = sortBy == "KeyCode" ? "KeyCodeDESC" : "KeyCode";
            ViewData["KeyStatus"] = sortBy == "KeyStatus" ? "KeyStatusDESC" : "KeyStatus";
            ViewData["Onwer"] = sortBy == "Onwer" ? "OnwerDESC" : "Onwer";

            if (search != null)
            {
                pageNumber = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewData["CurrentFilter"] = search;

            var productKeys = _context.ProductKey.Include(p => p.ProductName).AsQueryable();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                productKeys = productKeys.Where(p => p.ProductName.Name.ToLower().Contains(search.ToLower()));
            }

            // Sort the product by name
            switch (sortBy)
            {
                case "KeyNameDESC":
                    productKeys = productKeys.OrderByDescending(p => p.ProductName.Name);
                    break;
                case "KeyCodeDESC":
                    productKeys = productKeys.OrderByDescending(p => p.Key);
                    break;
                case "KeyCode":
                    productKeys = productKeys.OrderBy(p => p.Key);
                    break;
                case "KeyStatusDESC":
                    productKeys = productKeys.OrderByDescending(p => p.Status);
                    break;
                case "KeyStatus":
                    productKeys = productKeys.OrderBy(p => p.Status);
                    break;
                case "OnwerDESC":
                    productKeys = productKeys.OrderByDescending(p => p.OwnerId);
                    break;
                case "Onwer":
                    productKeys = productKeys.OrderBy(p => p.OwnerId);
                    break;
                default:
                    productKeys = productKeys.OrderBy(p => p.ProductName.Name);
                    break;
            }

            var model = await PaginatedList<ProductKey>.CreateAsync(productKeys.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(model);
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
            if (file == null || file.Length == 0)
                return RedirectToAction("Index");

            var parser = new CsvParser(file, ';');
            var results = parser.ParseKeys();

            foreach (Tuple<string, string> pk in results.ValidList)
            {
                ProductKey existingRow = _context.ProductKey
                    .Include(p => p.ProductName)
                    .FirstOrDefault(p => p.ProductName.Name == pk.Item1
                                      && p.Key == pk.Item2);

                if (existingRow == null)
                {
                    var productName = _context.ProductName.FirstOrDefault(pn => pn.Name == pk.Item1);

                    if (productName != null)
                    {
                        var nameId = productName.Id;
                        ProductKey newKey = new ProductKey()
                        {
                            NameId = nameId,
                            //ProductName = productName,
                            Key = pk.Item2,
                            Status = "New"
                        };
                        _context.ProductKey.Add(newKey);
                    }

                    //TODO: Handle key when productName not found
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
