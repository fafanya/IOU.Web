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
    public class UBillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UBillsController(ApplicationDbContext context)
        {
            _context = context;    
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UBills.Include(u => u.UEvent).Include(u => u.UMember);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);
            if (uBill == null)
            {
                return NotFound();
            }

            return View(uBill);
        }
        public IActionResult Create(int? uEventId)
        {
            UEvent uEvent = _context.UEvents.FirstOrDefault(x => x.Id == uEventId);
            ViewData["UEventTypeId"] = uEvent.UEventTypeId;
            ViewData["UEventId"] = uEventId;
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Name");
            return View();
        }

        // POST: UBills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,UEventId,UMemberId")] UBill uBill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uBill);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "UEvents", new { id = uBill.UEventId });
            }
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Name", uBill.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Name", uBill.UMemberId);
            return View(uBill);
        }

        // GET: UBills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);
            if (uBill == null)
            {
                return NotFound();
            }
            UEvent uEvent = _context.UEvents.FirstOrDefault(x => x.Id == uBill.UEventId);
            ViewData["UEventTypeId"] = uEvent.UEventTypeId;
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Id", uBill.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Id", uBill.UMemberId);
            return View(uBill);
        }

        // POST: UBills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,UEventId,UMemberId")] UBill uBill)
        {
            if (id != uBill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uBill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UBillExists(uBill.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "UEvents", new { id = uBill.UEventId });
            }
            ViewData["UEventId"] = new SelectList(_context.UEvents, "Id", "Id", uBill.UEventId);
            ViewData["UMemberId"] = new SelectList(_context.UMembers, "Id", "Id", uBill.UMemberId);
            return View(uBill);
        }

        // GET: UBills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);
            if (uBill == null)
            {
                return NotFound();
            }

            return View(uBill);
        }

        // POST: UBills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);
            _context.UBills.Remove(uBill);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "UEvents", new { id = uBill.UEventId });
        }

        private bool UBillExists(int id)
        {
            return _context.UBills.Any(e => e.Id == id);
        }
    }
}
