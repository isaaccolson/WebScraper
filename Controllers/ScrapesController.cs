using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockScraper.Data;
using StockScraper.Models;
     
namespace StockScraper.Controllers
{

    public class ScrapesController : Controller
    {
        private readonly ApplicationDbContext _context;

      
        public ScrapesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> StartScraper()
        
        {
            return View();
        }

        public IActionResult Retrospective()
        {
            return View();
        }

        // GET: Scrapes
        [Authorize]
        public async Task<IActionResult> Index(DateTime searchDate)
        {
            IQueryable<DateTime> dateQuery = from m in _context.Scrape
                                            orderby m.Date
                                            select m.Date;

            var scrape = from m in _context.Scrape
                         select m;

            if (!String.IsNullOrEmpty(searchDate.ToString()))
            {
              //  scrape = scrape.Where(s => s.Date == searchDate);
            }

            var scrapeDateVM = new ScrapeDateViewModel {

                Dates = new SelectList(await dateQuery.Distinct().ToListAsync()),
                Scrapes = await scrape.ToListAsync()
            };

            return View(scrapeDateVM);
        }

        // GET: Scrapes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrape = await _context.Scrape
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scrape == null)
            {
                return NotFound();
            }

            return View(scrape);
        }

        // GET: Scrapes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Scrapes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Change,Volume,DayRange,Date")] Scrape scrape)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scrape);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scrape);
        }

        // GET: Scrapes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrape = await _context.Scrape.FindAsync(id);
            if (scrape == null)
            {
                return NotFound();
            }
            return View(scrape);
        }

        // POST: Scrapes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Change,Volume,DayRange,Date")] Scrape scrape)
        {
            if (id != scrape.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scrape);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScrapeExists(scrape.Id))
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
            return View(scrape);
        }

        // GET: Scrapes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrape = await _context.Scrape
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scrape == null)
            {
                return NotFound();
            }

            return View(scrape);
        }

        // POST: Scrapes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scrape = await _context.Scrape.FindAsync(id);
            _context.Scrape.Remove(scrape);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScrapeExists(int id)
        {
            return _context.Scrape.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Scrape() {

            var seleniumScraper = new SeleniumScraper("isaac.colson@gmail.com", "J18109534I$@@c");
            seleniumScraper.Run();

            return View("Index");
        }
    }
}
