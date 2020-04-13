using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSL_APP.Data;
using MSL_APP.Models;
using MSL_APP.Utility;

namespace MSL_APP.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        List<SelectListItem> activeStatus = new List<SelectListItem> 
        {
            new SelectListItem { Text = "Active", Value = "Active" },
            new SelectListItem { Text = "Disable",  Value = "Disable" },
        };
        public ProductController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Product
        [Authorize(Roles = "Admin, Student")]
        public async Task<IActionResult> Student(string sortBy, string search, string currentFilter, int? pageNumber, int? pageRow)
        {
            int pageSize = pageRow ?? 10;
            ViewData["totalRow"] = pageRow;
            ViewData["CurrentSort"] = sortBy;
            ViewData["StudentProduct"] = string.IsNullOrEmpty(sortBy) ? "NameDESC" : "";
            ViewData["StudentLink"] = sortBy == "DownloadLink" ? "DownloadLinkDESC" : "DownloadLink";

            if (search != null)
            {
                pageNumber = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewData["CurrentFilter"] = search;

            // Display only actived products
            var studentProducts = _context.Product.Where(p => p.ActiveStatus == "Active");

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                studentProducts = studentProducts.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            // Sort the product by name
            switch (sortBy)
            {
                case "NameDESC":
                    studentProducts = studentProducts.OrderByDescending(p => p.Name);
                    break;
                case "DownloadLinkDESC":
                    studentProducts = studentProducts.OrderByDescending(p => p.DownloadLink);
                    break;
                case "DownloadLink":
                    studentProducts = studentProducts.OrderBy(p => p.DownloadLink);
                    break;
                default:
                    studentProducts = studentProducts.OrderBy(p => p.Name);
                    break;
            }

            if (pageRow == -1)
            {
                pageSize = studentProducts.Count();
                ViewData["totalRow"] = pageSize;
            }

            var model = await PaginatedList<Product>.CreateAsync(studentProducts.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(model);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin, Student")]
        public IActionResult GetKey(int id)
        {
            string productKey = "";

            // get current logged in user's student id
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userStudentId = -1;
            string userStudentEmail = "";
            var users = _userManager.Users.AsQueryable();
            var findUser = users.Where(u => u.Id == userId).FirstOrDefault();
            if (findUser != null) {
                userStudentId = findUser.StudentId;
                userStudentEmail = findUser.Email;
            }

            var products = _context.Product.AsQueryable();
            // Get the selected product info
            var product = products.Where(p => p.Id == id).FirstOrDefault();
            // Get the product name
            string productName = product.Name;
            // Get the quantity allowed for this product
            int quantityLimit = product.QuantityLimit;
            // Get all the keys for the selected product
            var productkeys = _context.ProductKey.Where(k => k.NameId == product.Id).OrderByDescending(k => k.Status).AsQueryable();

            // Need to check the student is eligible in order to get the key
            var eligibleStudents = _context.EligibleStudent.AsQueryable();
            bool isEligible = eligibleStudents.Any(e => e.StudentID == userStudentId);
            // Student is eligible to get new key
            if (isEligible) {
                // Check the student has exceed the acquirable quantity limit or not
                int ownedKeyCount = productkeys.Where(k => k.OwnerId == userStudentId).Count();
                bool foundKey = false;
                // Student has not exceed the aquirable quantity limit
                if (ownedKeyCount < quantityLimit)
                {
                    // Find an available key for student
                    foreach (ProductKey key in productkeys)
                    {
                        if (key.Status == "New")
                        {
                            productKey = key.Key;
                            key.Status = "Used";
                            key.OwnerId = userStudentId;
                            foundKey = true;
                            break;
                        }
                    }
                    // Found a new key in the database
                    if (foundKey)
                    {
                        // update the used key count
                        product.UsedKeyCount += 1;
                        _context.Entry(product).Property("UsedKeyCount").IsModified = true;
                        _context.SaveChanges();

                        // Write log for get key action
                        Logs newLog = new Logs()
                        {
                            StudentId = userStudentId,
                            StudentEmail = userStudentEmail,
                            Action = "GetKey",
                            Product = productName,
                            ProductKey = productKey,
                        };
                        _context.Add(newLog);
                        _context.SaveChanges();

                        ViewData["StudentProductName"] = productName;
                        ViewData["StudentProductKey"] = productKey;
                        ViewData["StudentGetKeySucceed"] = true;
                        return View();
                    }
                    // The key of selected product is out of stock
                    else
                    {
                        ViewData["StudentProductName"] = productName;
                        ViewData["StudentGetKeySucceed"] = false;
                        ViewData["StudentGetKeyMessage"] = "This key is out of stock. Please contact the administrator for more information.";
                        return View();
                    }
                }
                // Student has exceed the acquirable quantity limit
                ViewData["StudentProductName"] = productName;
                ViewData["StudentGetKeySucceed"] = false;
                ViewData["StudentGetKeyMessage"] = "You have reached the maximum number of keys for this product.";
                return View();
            }
            // Student is not eligible to get new key
            ViewData["StudentProductName"] = productName;
            ViewData["StudentGetKeySucceed"] = false;
            ViewData["StudentGetKeyMessage"] = "You are no longer eligible to claim new keys. Please contact the administrator for more information.";
            return View();

        }


        // GET: Product
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortBy, string search, string currentFilter, int? pageNumber, int? pageRow)
        {
            int pageSize = pageRow ?? 10;
            ViewData["totalRow"] = pageRow;
            ViewData["CurrentSort"] = sortBy;
            ViewData["Product"] = string.IsNullOrEmpty(sortBy) ? "NameDESC" : "";
            ViewData["TotalKeys"] = sortBy == "TotalKey" ? "TotalKeyDESC" : "TotalKey";
            ViewData["Limit"] = sortBy == "QuantityLimit" ? "QuantityLimitDESC" : "QuantityLimit";
            ViewData["Status"] = sortBy == "ActiveStatus" ? "ActiveStatusDESC" : "ActiveStatus";
            ViewData["Link"] = sortBy == "DownloadLink" ? "DownloadLinkDESC" : "DownloadLink";

            if (search != null)
            {
                pageNumber = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewData["CurrentFilter"] = search;

            var products = _context.Product.AsQueryable();
            var productkeys = _context.ProductKey.AsQueryable();

            //// Count the key number for each product and store the number into database
            //foreach (Product product in products) {
            //    int keyCount = productkeys.Where(k => k.NameId == product.Id).Count();
            //    int usedKeyCount = productkeys.Where(k => k.NameId == product.Id && k.Status == "Used").Count();
            //    // Save the calculated key count number into database
            //    product.KeyCount = keyCount;
            //    _context.Entry(product).Property("KeyCount").IsModified = true;

            //    product.UsedKeyCount = usedKeyCount;
            //    _context.Entry(product).Property("UsedKeyCount").IsModified = true;

            //}
            //_context.SaveChanges();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

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

            if (pageRow == -1)
            {
                pageSize = products.Count();
                ViewData["totalRow"] = pageSize;
            }

            var model = await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(model);
        }

        // GET: Product/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.productstatus = activeStatus;
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,QuantityLimit,KeyCount,UsedKeyCount,ActiveStatus,DownloadLink")] Product product)
        {
            ViewBag.productstatus = activeStatus;

            //Check if given name is a duplicate
            if (_context.Product.Any(p => p.Name == product.Name))
            {
                ModelState.AddModelError("Name", "Product name already exists");
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.productstatus = activeStatus;

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,QuantityLimit,ActiveStatus,DownloadLink")] Product product)
        {
            ViewBag.productstatus = activeStatus;

            if (id != product.Id)
            {
                return NotFound();
            }

            //Check if given name is a duplicate
            if (_context.Product.Any(p => p.Name == product.Name && p.Id != id))
            {
                ModelState.AddModelError("Name", "Product name already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // _context.Update(product);
                    _context.Entry(product).Property("Name").IsModified = true;
                    _context.Entry(product).Property("QuantityLimit").IsModified = true;
                    _context.Entry(product).Property("ActiveStatus").IsModified = true;
                    _context.Entry(product).Property("DownloadLink").IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductNameExists(product.Id))
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
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductNameExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
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


            try
            { 
                var parser = new LicenseParser(file, ';');
                var results = parser.ParseProducts();
                foreach (KeyValuePair<string, Product> entry in results.ValidList)
                {
                    Product existingRow = _context.Product.FirstOrDefault(p => p.Name == entry.Value.Name);
                    if (existingRow == null)
                        _context.Product.Add(entry.Value);
                    else
                        results.InvalidList.Add(entry.Key, entry.Value.Name);
                }

                await _context.SaveChangesAsync();
                TempData["InvalidList"] = results.InvalidList;
            }
            catch (InvalidDataException e)
            {
                TempData["BadFileType"] = e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }

            return RedirectToAction("Index");
        }
    }
}
