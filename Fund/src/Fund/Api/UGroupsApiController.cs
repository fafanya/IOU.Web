using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;
using Fund.Models.UGroupViewModels;

using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace Fund.Api
{
    [Produces("application/json")]
    [Route("api/UGroupsApi")]
    public class UGroupsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UGroupsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginViewModel login)
        {
            UGroup g = _context.UGroups.SingleOrDefault(x => x.Id == login.UGroupId && x.Password == login.Password);
            if (g != null)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] UGroup uGroup)
        {
            UGroup newGroup = new UGroup();
            newGroup.Name = uGroup.Name;
            newGroup.UUserId = uGroup.UUserId;
            newGroup.UMembers = new List<UMember>();
            newGroup.UEvents = new List<UEvent>();
            newGroup.UDebts = new List<UDebt>();

            Dictionary<int, UMember> old2newMember = new Dictionary<int, UMember>();
            foreach (UMember m in uGroup.UMembers)
            {
                UMember newMember = new UMember();
                newMember.Name = m.Name;
                newMember.DebtorUDebts = new List<UDebt>();
                newMember.LenderUDebts = new List<UDebt>();
                newGroup.UMembers.Add(newMember);
                old2newMember.Add(m.Id, newMember);
            }

            foreach (UEvent e in uGroup.UEvents)
            {
                UEvent newEvent = new UEvent();
                newEvent.Name = e.Name;
                newEvent.UEventType = _context.UEventTypes.FirstOrDefault(x => x.Id == e.UEventTypeId);
                newEvent.UBills = new List<UBill>();
                newEvent.UPayments = new List<UPayment>();
                newGroup.UEvents.Add(newEvent);

                foreach (UBill b in e.UBills)
                {
                    UBill newBill = new UBill();
                    newBill.Amount = b.Amount;
                    newBill.UMember = old2newMember[b.UMemberId];
                    newEvent.UBills.Add(newBill);
                }
                foreach (UPayment p in e.UPayments)
                {
                    UPayment newPayment = new UPayment();
                    newPayment.Amount = p.Amount;
                    newPayment.UMember = old2newMember[p.UMemberId];
                    newEvent.UPayments.Add(newPayment);
                }
            }

            foreach (UDebt d in uGroup.UDebts)
            {
                UDebt newDebt = new UDebt();
                newDebt.Name = d.Name;
                newDebt.Amount = d.Amount;
                newDebt.Lender = old2newMember[d.LenderId];
                newDebt.Debtor = old2newMember[d.DebtorId];
                newGroup.UDebts.Add(newDebt);
            }
            _context.UGroups.Add(newGroup);
            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                var error = ex;
                return Json(false);
            }
            return Json(true);
        }

        // GET: api/UGroups
        [HttpGet("ByUser/{id}")]
        public IEnumerable<UGroup> ByUser([FromRoute] string id)
        {
            IEnumerable<UGroup> uGroups = _context.UGroups
                .Where(x => (x.UUserId == id) || (x.UMembers.Any(m => m.UUserId == id)));
            return uGroups;
        }

        [HttpGet("Import/{id}")]
        public async Task<IActionResult> Import([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UGroup g = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);
            g.UDebts = _context.UDebts.Where(x => x.UGroupId == id).ToList();
            g.UEvents = _context.UEvents.Where(x => x.UGroupId == id).ToList();
            g.UMembers = _context.UMembers.Where(x => x.UGroupId == id).ToList();
            foreach (UEvent e in g.UEvents)
            {
                e.UPayments = _context.UPayments.Where(x => x.UEventId == e.Id).ToList();
                e.UBills = _context.UBills.Where(x => x.UEventId == e.Id).ToList();
            }

            if (g == null)
            {
                return NotFound();
            }

            UGroup newGroup = new UGroup();
            newGroup.Name = g.Name;

            newGroup.UDebts = new List<UDebt>();
            newGroup.UMembers = new List<UMember>();
            newGroup.UEvents = new List<UEvent>();
            foreach(UDebt d in g.UDebts)
            {
                UDebt newDebt = new UDebt();
                newDebt.LenderId = d.LenderId;
                newDebt.DebtorId = d.DebtorId;
                newDebt.Name = d.Name;
                newDebt.Amount = d.Amount;

                newGroup.UDebts.Add(newDebt);
            }
            foreach(UMember m in g.UMembers)
            {
                UMember newMember = new UMember();
                newMember.Id = m.Id;
                newMember.Name = m.Name;
                newMember.UGroupId = m.UGroupId;

                newGroup.UMembers.Add(newMember);
            }
            foreach(UEvent e in g.UEvents)
            {
                UEvent newEvent = new UEvent();
                newEvent.Name = e.Name;
                newEvent.UEventTypeId = e.UEventTypeId;
                newEvent.UGroupId = e.UGroupId;
                newEvent.Id = e.Id;

                newEvent.UBills = new List<UBill>();
                newEvent.UPayments = new List<UPayment>();
                foreach(UBill b in e.UBills)
                {
                    UBill newBill = new UBill();
                    newBill.Amount = b.Amount;
                    newBill.UMemberId = b.UMemberId;

                    newEvent.UBills.Add(newBill);
                }
                foreach(UPayment p in e.UPayments)
                {
                    UPayment newPayment = new UPayment();
                    newPayment.Amount = p.Amount;
                    newPayment.UMemberId = p.UMemberId;

                    newEvent.UPayments.Add(newPayment);
                }
                newGroup.UEvents.Add(newEvent);
            }
            
            try
            {
                string result = JsonConvert.SerializeObject(newGroup);
            }
            catch(Exception ex)
            {
                var error = ex;
            }

            return Ok(newGroup);
        }

        // GET: api/UGroups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UGroup uGroup = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);

            if (uGroup == null)
            {
                return NotFound();
            }

            return Ok(uGroup);
        }

        // PUT: api/UGroups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUGroup([FromRoute] int id, [FromBody] UGroup uGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(uGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UGroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UGroups
        [HttpPost]
        public async Task<IActionResult> PostUGroup([FromBody] UGroup uGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UGroups.Add(uGroup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UGroupExists(uGroup.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUGroup", new { id = uGroup.Id }, uGroup);
        }

        // DELETE: api/UGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UGroup uGroup = await _context.UGroups.SingleOrDefaultAsync(m => m.Id == id);
            if (uGroup == null)
            {
                return NotFound();
            }

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
            catch(Exception ex)
            {
                var error = ex;
            }

            return Ok(uGroup);
        }

        private bool UGroupExists(int id)
        {
            return _context.UGroups.Any(e => e.Id == id);
        }

    }
}