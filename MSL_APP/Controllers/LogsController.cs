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
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Logs
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
            ViewData["PageIndex"] = pageNumber;
            var log = _context.Logs.OrderByDescending(l => l.TimeStamp).AsQueryable();

            // Search product by the input
            if (!string.IsNullOrEmpty(search))
            {
                log = log.Where(l => l.StudentEmail.ToLower().Contains(search.ToLower())
                || l.StudentId.ToString().Contains(search)
                || l.Product.Contains(search)
                || l.ProductKey.Contains(search)
                || l.Action.Contains(search)
                || l.TimeStamp.ToString("MM-dd-yyyy hh:mm:ss tt").Contains(search));
            }

            if (pageRow == -1)
            {
                pageSize = log.Count();
                ViewData["totalRow"] = pageSize;
            }

            var model = await PaginatedList<Logs>.CreateAsync(log.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(model);
        }


        // GET: Logs/Delete/5
        public async Task<IActionResult> Delete(int? id, string currentFilter, int? pageNumber, int? pageRow)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }
            else
            {
                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { currentFilter, pageNumber, pageRow });
            }
        }
    }
}
