using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSL_APP.Data;
using MSL_APP.Models;
using MSL_APP.Utility;

namespace MSL_APP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductKeyLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductKeyLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductKeyLog
        public async Task<IActionResult> Index(string search, string currentFilter, int? pageNumber, int? pageRow)
        {


            int pageSize = pageRow ?? 10;
            ViewData["totalRow"] = pageRow;

            if (search != null)
            {
                pageNumber = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewData["CurrentFilter"] = search;

            var log = _context.ProductKeyLog.OrderByDescending(l => l.TimeStamp).AsQueryable();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                log = log.Where(l => l.StudentEmail.ToLower().Contains(search.ToLower())
                || l.StudentId.ToString().Contains(search)
                || l.ProductName.Contains(search)
                || l.ProductKey.Contains(search)
                || l.Action.Contains(search));
            }

            if (pageRow == -1)
            {
                pageSize = log.Count();
                ViewData["totalRow"] = pageSize;
            }

            var model = await PaginatedList<ProductKeyLog>.CreateAsync(log.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(model);
        }


        // GET: ProductKeyLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
            else
            {
                _context.ProductKeyLog.Remove(productKeyLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
