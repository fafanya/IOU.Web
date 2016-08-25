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
    public class UDebtsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UDebtsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: UDebts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UDebts.Include(u=>u.Debtor).Include(u=>u.Lender).Include(u => u.UGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UDebts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);
            if (uDebt == null)
            {
                return NotFound();
            }

            return View(uDebt);
        }

        // GET: UDebts/Create
        public IActionResult Create(int? uGroupId)
        {
            ViewData["UGroupId"] = uGroupId;
            ViewData["DebtorId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uGroupId), "Id", "Name");
            ViewData["LenderId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uGroupId), "Id", "Name");
            return View();
        }

        // POST: UDebts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,DebtorId,LenderId,Name,UGroupId")] UDebt uDebt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uDebt);
                await _context.SaveChangesAsync();
                //return RedirectToAction("Index");
                return RedirectToAction("DetailsDebts", "UGroups", new { id = uDebt.UGroupId });
            }
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uDebt.UGroupId);
            ViewData["DebtorId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uDebt.UGroupId), "Id",
                "Name", uDebt.DebtorId);
            ViewData["LenderId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uDebt.UGroupId), "Id",
                "Name", uDebt.LenderId);
            return View(uDebt);
        }

        // GET: UDebts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);
            if (uDebt == null)
            {
                return NotFound();
            }
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uDebt.UGroupId);
            return View(uDebt);
        }

        // POST: UDebts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,DebtorId,LenderId,Name,UGroupId")] UDebt uDebt)
        {
            if (id != uDebt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uDebt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UDebtExists(uDebt.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsDebts", "UGroups", new { id = uDebt.UGroupId });
            }
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uDebt.UGroupId);
            ViewData["DebtorId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uDebt.UGroupId), "Id",
                "Name", uDebt.DebtorId);
            ViewData["LenderId"] = new SelectList(_context.UMembers.Where(u => u.UGroupId == uDebt.UGroupId), "Id",
                "Name", uDebt.LenderId);
            return View(uDebt);
        }

        // GET: UDebts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);
            if (uDebt == null)
            {
                return NotFound();
            }

            return View(uDebt);
        }

        // POST: UDebts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);
            _context.UDebts.Remove(uDebt);
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsDebts", "UGroups", new { id = uDebt.UGroupId });
        }

        private bool UDebtExists(int id)
        {
            return _context.UDebts.Any(e => e.Id == id);
        }
    }
}
