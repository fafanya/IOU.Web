using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;

namespace Fund.Api
{
    [Produces("application/json")]
    [Route("api/UBillsApi")]
    public class UBillsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UBillsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ByEvent/{id}")]
        public IEnumerable<UBill> ByEvent([FromRoute] int id)
        {
            IEnumerable<UBill> uBills = _context.UBills.Where(x => x.UEventId == id);

            List<UBill> result = new List<UBill>();
            foreach (UBill uBill in uBills)
            {
                UBill b = new UBill();
                b.Amount = uBill.Amount;
                b.UEventId = uBill.UEventId;
                b.UMemberId = uBill.UMemberId;
                b.Id = uBill.Id;

                b.MemberName = _context.UMembers.FirstOrDefault(x => x.Id == b.UMemberId).Name;

                result.Add(b);
            }
            return result;
        }

        // GET: api/UBills
        [HttpGet]
        public IEnumerable<UBill> GetUBills()
        {
            return _context.UBills;
        }

        // GET: api/UBills/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUBill([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UBill uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);

            if (uBill == null)
            {
                return NotFound();
            }

            return Ok(uBill);
        }

        // PUT: api/UBills/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUBill([FromRoute] int id, [FromBody] UBill uBill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uBill.Id)
            {
                return BadRequest();
            }

            _context.Entry(uBill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UBillExists(id))
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

        // POST: api/UBills
        [HttpPost]
        public async Task<IActionResult> PostUBill([FromBody] UBill uBill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UBills.Add(uBill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UBillExists(uBill.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUBill", new { id = uBill.Id }, uBill);
        }

        // DELETE: api/UBills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUBill([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UBill uBill = await _context.UBills.SingleOrDefaultAsync(m => m.Id == id);
            if (uBill == null)
            {
                return NotFound();
            }

            _context.UBills.Remove(uBill);
            await _context.SaveChangesAsync();

            return Ok(uBill);
        }

        private bool UBillExists(int id)
        {
            return _context.UBills.Any(e => e.Id == id);
        }
    }
}