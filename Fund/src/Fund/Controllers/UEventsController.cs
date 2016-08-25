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
    public class UEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UEventsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: UEvents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UEvents.Include(u => u.UEventType).Include(u => u.UGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ByGroup(int? id)
        {
            var applicationDbContext = _context.UEvents.Where(x=>x.UGroupId == id).Include(u => u.UEventType).Include(u => u.UGroup);
            return View("Index", await applicationDbContext.ToListAsync());
        }

        // GET: UEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uEvent = await _context.UEvents
                .Include(u=>u.UBills)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (uEvent == null)
            {
                return NotFound();
            }

            if (uEvent.UBills != null)
            {
                foreach (UBill b in uEvent.UBills)
                {
                    b.UMember = _context.UMembers.FirstOrDefault(x => x.Id == b.UMemberId);
                }
            }

            if (uEvent.UEventTypeId == UEventType.tCommon)
            {
                return RedirectToAction("DetailsPayments", new { id = uEvent.Id });
            }

            return View(uEvent);
        }

        public async Task<IActionResult> DetailsPayments(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uEvent = await _context.UEvents
                .Include(u => u.UPayments)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (uEvent == null)
            {
                return NotFound();
            }

            if (uEvent.UPayments != null)
            {
                foreach (UPayment p in uEvent.UPayments)
                {
                    p.UMember = _context.UMembers.FirstOrDefault(x => x.Id == p.UMemberId);
                }
            }

            return View(uEvent);
        }

        public async Task<IActionResult> DetailsTotals(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uEvent = await _context.UEvents
                .SingleOrDefaultAsync(m => m.Id == id);
            uEvent.UTotals = GetUTotals(uEvent);
            if (uEvent == null)
            {
                return NotFound();
            }

            return View(uEvent);
        }

        // GET: UEvents/Create
        public IActionResult Create(int? uGroupId)
        {
            ViewData["UEventTypeId"] = new SelectList(_context.UEventTypes, "Id", "Name");
            ViewData["UGroupId"] = uGroupId;
            return View();
        }

        // POST: UEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UEventTypeId,UGroupId")] UEvent uEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsEvents", "UGroups", new { id = uEvent.UGroupId });
            }
            ViewData["UEventTypeId"] = new SelectList(_context.UEventTypes, "Id", "Name", uEvent.UEventTypeId);
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Name", uEvent.UGroupId);
            return View(uEvent);
        }

        // GET: UEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uEvent = await _context.UEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (uEvent == null)
            {
                return NotFound();
            }
            ViewData["UEventTypeId"] = new SelectList(_context.UEventTypes, "Id", "Id", uEvent.UEventTypeId);
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uEvent.UGroupId);
            return View(uEvent);
        }

        // POST: UEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UEventTypeId,UGroupId")] UEvent uEvent)
        {
            if (id != uEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UEventExists(uEvent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsEvents", "UGroups", new { id = uEvent.UGroupId });
            }
            ViewData["UEventTypeId"] = new SelectList(_context.UEventTypes, "Id", "Id", uEvent.UEventTypeId);
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uEvent.UGroupId);
            return View(uEvent);
        }

        // GET: UEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uEvent = await _context.UEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (uEvent == null)
            {
                return NotFound();
            }

            return View(uEvent);
        }

        // POST: UEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uEvent = await _context.UEvents.SingleOrDefaultAsync(m => m.Id == id);

            var payments = _context.UPayments.Where(x => x.UEventId == uEvent.Id).ToList();
            _context.RemoveRange(payments);
            var bills = _context.UBills.Where(x => x.UEventId == uEvent.Id).ToList();
            _context.RemoveRange(bills);

            _context.UEvents.Remove(uEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsEvents", "UGroups", new { id = uEvent.UGroupId });
        }

        private bool UEventExists(int id)
        {
            return _context.UEvents.Any(e => e.Id == id);
        }

        private List<UTotal> GetUTotals(UEvent e)
        {
            List<UMember> uMembers = _context.UMembers.Where(x => x.UGroupId == e.UGroupId).ToList();

            if (e.UEventTypeId == UEventType.tOwn)
            {
                foreach (UBill b in _context.UBills.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                {
                    b.UMember.Balance += b.Amount;
                }
                foreach (UPayment p in _context.UPayments.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                {
                    p.UMember.Balance -= p.Amount;
                }
            }
            else if (e.UEventTypeId == UEventType.tCommon)
            {
                foreach (UPayment p in _context.UPayments.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                {
                    p.UMember.Balance -= p.Amount;
                }

                double avg = _context.UPayments.Where(x => x.UEventId == e.Id).Sum(x => x.Amount) / uMembers.Count;
                foreach (UMember m in uMembers)
                {
                    m.Balance += avg;
                }
            }
            else if (e.UEventTypeId == UEventType.tPartly)
            {
                int count = _context.UBills.Where(x => x.UEventId == e.Id).GroupBy(x => x.UMemberId).Count();
                double avg = _context.UPayments.Where(x => x.UEventId == e.Id).Sum(x => x.Amount) / count;
                foreach (UBill b in _context.UBills.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                {
                    b.UMember.Balance += avg;
                }

                foreach (UPayment p in _context.UPayments.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                {
                    p.UMember.Balance -= p.Amount;
                }
            }

            return RecountTotalDebtList(uMembers);
        }

        public static List<UTotal> RecountTotalDebtList(List<UMember> b)
        {
            List<UTotal> totals = new List<UTotal>();

            var N = b.Count;

            var i = 0;
            var j = 0;
            double m = 0;

            while (i != N && j != N)
            {
                if (b[i].Balance <= 0)
                {
                    i = i + 1;
                }
                else if (b[j].Balance >= 0)
                {
                    j = j + 1;
                }
                else
                {
                    if (b[i].Balance < -b[j].Balance)
                    {
                        m = b[i].Balance;
                    }
                    else
                    {
                        m = -b[j].Balance;
                    }
                    UTotal debt = new UTotal();
                    debt.DebtorName = b[i].Name;
                    debt.LenderName = b[j].Name;
                    debt.Amount = m;
                    totals.Add(debt);
                    b[i].Balance = b[i].Balance - m;
                    b[j].Balance = b[j].Balance + m;
                }
            }

            return totals;
        }
    }
}
