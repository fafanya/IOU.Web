using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fund.Data;
using Fund.Models;
using Microsoft.EntityFrameworkCore;

namespace Fund.Api
{
    [Produces("application/json")]
    [Route("api/UTotalsApi")]
    public class UTotalsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UTotalsApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/UTotalsApi
        [HttpGet("ByGroup/{id}")]
        public IEnumerable<UTotal> ByGroup(int id)
        {
            UGroup uGroup = _context.UGroups
                .Include(u => u.UEvents)
                .Include(u => u.UMembers)
                .Include(u => u.UDebts)
                .Include(u => u.UUser)
                .SingleOrDefault(m => m.Id == id);

            foreach (UDebt d in _context.UDebts.Include(x => x.Debtor).Include(x => x.Lender).Where(x => x.UGroupId == uGroup.Id))
            {
                d.Lender.Balance -= d.Amount;
                d.Debtor.Balance += d.Amount;
            }

            foreach (UEvent e in uGroup.UEvents)
            {
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

                    double avg = _context.UPayments.Where(x => x.UEventId == e.Id).Sum(x => x.Amount) / uGroup.UMembers.Count;
                    foreach (UMember m in uGroup.UMembers)
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
            }

            return RecountTotalDebtList(uGroup.UMembers);
        }

        [HttpGet("ByEvent/{id}")]
        public IEnumerable<UTotal> ByEvent(int id)
        {
            UEvent e = _context.UEvents
                .Include(u => u.UBills)
                .Include(u => u.UPayments)
                .SingleOrDefault(m => m.Id == id);

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
