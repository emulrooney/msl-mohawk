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
        public async Task<IActionResult> Index(string sortBy, string search, string currentFilter, int? pageNumber, int? pageRow)
        {
            int pageSize = pageRow ?? 10;
            ViewData["totalRow"] = pageRow;
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

            var productKeys = _context.ProductKey.Include(p => p.Product).AsQueryable();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                productKeys = productKeys.Where(p => p.Product.Name.ToLower().Contains(search.ToLower()));
            }

            // Sort the product by name
            switch (sortBy)
            {
                case "KeyNameDESC":
                    productKeys = productKeys.OrderByDescending(p => p.Product.Name);
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
                    productKeys = productKeys.OrderBy(p => p.Product.Name);
                    break;
            }

            if (pageRow == -1)
            {
                pageSize = productKeys.Count();
                ViewData["totalRow"] = pageSize;
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
                .Include(p => p.Product)
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
            ViewData["NameId"] = new SelectList(_context.Product, "Id", "Name");
            return View();
        }

        // POST: ProductKey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameId,Key,Status,OwnerId")] ProductKey productKey)
        {
            ViewBag.keystatus = status;
            //Check if given key is a duplicate
            if (_context.ProductKey.Any(e => e.Key == productKey.Key))
            {
                ModelState.AddModelError("Key", "Key already exists");
            }

            if (ModelState.IsValid)
            {
                // Increse current product key count by one

                _context.Add(productKey);
                await _context.SaveChangesAsync();

                // Add the total key count by one
                var products = _context.Product.AsQueryable();
                var product = products.Where(p => p.Id == productKey.NameId).FirstOrDefault();
                if (product != null) {
                    product.KeyCount += 1;
                    _context.Entry(product).Property("KeyCount").IsModified = true;
                }
                // Add used key count by one if the status of created key is used
                if (productKey.Status == "Used") {
                    product.UsedKeyCount+=1;
                    _context.Entry(product).Property("UsedKeyCount").IsModified = true;
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["NameId"] = new SelectList(_context.Product, "Id", "Name", productKey.NameId);
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
            ViewData["NameId"] = new SelectList(_context.Product, "Id", "Name", productKey.NameId);
            return View(productKey);
        }

        // POST: ProductKey/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameId,Key,Status,OwnerId")] ProductKey productKey)
        {
            ViewBag.keystatus = status;
            if (id != productKey.Id)
            {
                return NotFound();
            }

            //Check if given key is a duplicate
            if (_context.ProductKey.Any(p => p.Key == productKey.Key && p.Id != id))
            {
                ModelState.AddModelError("Key", "Key already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Edit the used key count
                    var products = _context.Product.AsQueryable();
                    var product = products.Where(p => p.Id == productKey.NameId).FirstOrDefault();

                    // Get the key's old status
                    var currentKey = _context.ProductKey.AsNoTracking().AsQueryable().Where(k => k.Id == id).FirstOrDefault();
                    string oldStatus = currentKey.Status;

                    // Add or minus used key count by one if the status of edited key is changed
                    if (oldStatus == "New" && productKey.Status == "Used")
                    {
                        product.UsedKeyCount++;
                        _context.Entry(product).Property("UsedKeyCount").IsModified = true;
                    } 
                    else if (oldStatus == "Used" && productKey.Status == "New") 
                    {
                        product.UsedKeyCount--;
                        _context.Entry(product).Property("UsedKeyCount").IsModified = true;
                    }
                    await _context.SaveChangesAsync();

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
            ViewData["NameId"] = new SelectList(_context.Product, "Id", "Name", productKey.NameId);
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
                .Include(p => p.Product)
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

            // Minus the total key count by one
            var products = _context.Product.AsQueryable();
            var product = products.Where(p => p.Id == productKey.NameId).FirstOrDefault();
            product.KeyCount-=1;
            _context.Entry(product).Property("KeyCount").IsModified = true;
            // Add used key count by one if the status of created key is used
            if (productKey.Status == "Used")
            {
                product.UsedKeyCount-=1;
                _context.Entry(product).Property("UsedKeyCount").IsModified = true;
            }
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

            try
            {
                var parser = new LicenseParser(file, ';');
                var results = parser.ParseKeys();

                foreach (Tuple<string, string> pk in results.ValidList)
                {
                    ProductKey existingRow = _context.ProductKey
                        .Include(p => p.Product)
                        .FirstOrDefault(p => p.Product.Name == pk.Item1
                                          && p.Key == pk.Item2);

                    if (existingRow == null)
                    {
                        var product = _context.Product.FirstOrDefault(pn => pn.Name == pk.Item1);

                        if (product != null)
                        {
                            var nameId = product.Id;
                            ProductKey newKey = new ProductKey()
                            {
                                NameId = nameId,
                                Key = pk.Item2,
                                Status = "New"
                            };
                            _context.ProductKey.Add(newKey);
                        }

                        //TODO: Handle key when product not found
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }

            // Count the key number for each product and store the number into database
            var products = _context.Product.AsQueryable();
            var productkeys = _context.ProductKey.AsQueryable();

            foreach (Product product in products)
            {
                int keyCount = productkeys.Where(k => k.NameId == product.Id).Count();
                int usedKeyCount = productkeys.Where(k => k.NameId == product.Id && k.Status == "Used").Count();
                // Save the calculated key count number into database
                product.KeyCount = keyCount;
                _context.Entry(product).Property("KeyCount").IsModified = true;

                product.UsedKeyCount = usedKeyCount;
                _context.Entry(product).Property("UsedKeyCount").IsModified = true;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
