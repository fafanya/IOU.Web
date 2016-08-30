using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;
using Fund.Models.UGroupViewModels;

namespace Fund.Controllers
{
    public class UGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UGroupsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: UGroups
        public IActionResult Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<UGroup> uGroups = _context.UGroups.Include(u => u.UMembers).Include(u => u.UUser)
                .Where(x => (x.UUserId == userId) || (x.UMembers.Any(m => m.UUserId == userId)));

            return View(uGroups);
        }

        // GET: UGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups
                .Include(u=>u.UMembers)
                .Include(u=>u.UUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            foreach(UMember m in uGroup.UMembers)
            {
                m.UUser = _context.UUsers.SingleOrDefault(x => x.Id == m.UUserId);
            }

            if (uGroup == null)
            {
                return NotFound();
            }

            return View(uGroup);
        }
        public async Task<IActionResult> DetailsDebts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups
                .Include(u => u.UDebts)
                .Include(u => u.UUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (uGroup == null)
            {
                return NotFound();
            }

            return View(uGroup);
        }
        public async Task<IActionResult> DetailsEvents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups
                .Include(u => u.UEvents)
                .Include(u => u.UUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (uGroup == null)
            {
                return NotFound();
            }

            return View(uGroup);
        }
        public async Task<IActionResult> DetailsTotals(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups
                .Include(u => u.UEvents)
                .Include(u => u.UMembers)
                .Include(u => u.UDebts)
                .Include(u => u.UUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            uGroup.UTotals = GetUTotals(uGroup.Id);
            if (uGroup == null)
            {
                return NotFound();
            }

            return View(uGroup);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login([Bind("UGroupId, Password")] LoginViewModel login)
        {
            UGroup g = _context.UGroups.SingleOrDefault(x => x.Id == login.UGroupId && x.Password == login.Password);
            if(g != null)
            {
                return RedirectToAction("Select", "UMembers", new { id = g.Id });
            }

            return RedirectToAction("Login");
        }

        // GET: UGroups/Create
        public IActionResult Create()
        {
            ViewData["UUserId"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        // POST: UGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UUserId,Password")] UGroup uGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["UUserId"] = new SelectList(_context.UUsers, "Id", "Name", uGroup.UUserId);
            return View(uGroup);
        }

        // GET: UGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);
            if (uGroup == null)
            {
                return NotFound();
            }
            return View(uGroup);
        }

        // POST: UGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password, UUserId")] UGroup uGroup)
        {
            if (id != uGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UGroupExists(uGroup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(uGroup);
        }

        // GET: UGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uGroup = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);
            if (uGroup == null)
            {
                return NotFound();
            }

            return View(uGroup);
        }

        // POST: UGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uGroup = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);

            var debts = _context.UDebts.Where(x => x.UGroupId == uGroup.Id).ToList();
            var events = _context.UEvents.Where(x => x.UGroupId == uGroup.Id).ToList();
            var members = _context.UMembers.Where(x => x.UGroupId == uGroup.Id).ToList();

            foreach (UEvent e in events)
            {
                var payments = _context.UPayments.Where(x => x.UEventId == e.Id).ToList();
                _context.RemoveRange(payments);
                var bills = _context.UBills.Where(x => x.UEventId == e.Id).ToList();
                _context.RemoveRange(bills);
            }
            _context.RemoveRange(debts);
            _context.RemoveRange(members);
            _context.RemoveRange(events);

            _context.UGroups.Remove(uGroup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = ex;
            }

            return RedirectToAction("Index");
        }

        private bool UGroupExists(int id)
        {
            return _context.UGroups.Any(e => e.Id == id);
        }

        public List<UTotal> GetUTotals(int id)
        {
            UGroup uGroup = _context.UGroups
                .Include(u => u.UEvents)
                .Include(u => u.UMembers)
                .Include(u => u.UDebts)
                .Include(u => u.UUser)
                .SingleOrDefault(m => m.Id == id);

            foreach (UDebt d in _context.UDebts.Include(x=>x.Debtor).Include(x=>x.Lender).Where(x=>x.UGroupId == uGroup.Id))
            {
                d.Lender.Balance -= d.Amount;
                d.Debtor.Balance += d.Amount;
            }

            foreach(UEvent e in uGroup.UEvents)
            {
                if(e.UEventTypeId == UEventType.tOwn)
                {
                    foreach(UBill b in _context.UBills.Include(x=>x.UMember).Where(x => x.UEventId == e.Id))
                    {
                        b.UMember.Balance += b.Amount;
                    }
                    foreach(UPayment p in _context.UPayments.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                    {
                        p.UMember.Balance -= p.Amount;
                    }
                }
                else if(e.UEventTypeId == UEventType.tCommon)
                {
                    foreach (UPayment p in _context.UPayments.Include(x => x.UMember).Where(x => x.UEventId == e.Id))
                    {
                        p.UMember.Balance -= p.Amount;
                    }

                    double avg = _context.UPayments.Where(x => x.UEventId == e.Id).Sum(x => x.Amount) / uGroup.UMembers.Count;
                    foreach(UMember m in uGroup.UMembers)
                    {
                        m.Balance += avg;
                    }
                }
                else if(e.UEventTypeId == UEventType.tPartly)
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
            }

            return RecountTotalDebtList(uGroup.UMembers);
        }

        private List<UTotal> RecountTotalDebtList(List<UMember> b)
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
