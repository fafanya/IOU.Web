using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;

namespace Fund.Controllers
{
    public class UPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UPaymentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: UPayments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UPayments.Include(u => u.UEvent).Include(u => u.UMember);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);
            if (uPayment == null)
            {
                return NotFound();
            }

            return View(uPayment);
        }

        // GET: UPayments/Create
        public IActionResult Create(int? uEventId)
        {
            ViewData["UEventId"] = uEventId;
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Name");
            return View();
        }

        // POST: UPayments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,UEventId,UMemberId")] UPayment uPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsPayments", "UEvents", new { id = uPayment.UEventId });
            }
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Id", uPayment.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Id", uPayment.UMemberId);
            return View(uPayment);
        }

        // GET: UPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);
            if (uPayment == null)
            {
                return NotFound();
            }
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Id", uPayment.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Id", uPayment.UMemberId);
            return View(uPayment);
        }

        // POST: UPayments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,UEventId,UMemberId")] UPayment uPayment)
        {
            if (id != uPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UPaymentExists(uPayment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsPayments", "UEvents", new { id = uPayment.UEventId });
            }
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Id", uPayment.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Id", uPayment.UMemberId);
            return View(uPayment);
        }

        // GET: UPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);
            if (uPayment == null)
            {
                return NotFound();
            }

            return View(uPayment);
        }

        // POST: UPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);
            _context.UPayments.Remove(uPayment);
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsPayments", "UEvents", new { id = uPayment.UEventId });
        }

        private bool UPaymentExists(int id)
        {
            return _context.UPayments.Any(e => e.Id == id);
        }
    }
}
