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
    [Route("api/UPaymentsApi")]
    public class UPaymentsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UPaymentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ByEvent/{id}")]
        public IEnumerable<UPayment> ByEvent([FromRoute] int id)
        {
            IEnumerable<UPayment> uPayments = _context.UPayments.Where(x => x.UEventId == id);
            foreach (UPayment uPayment in uPayments)
            {
                UPayment p = new UPayment();
                p.Amount = uPayment.Amount;
                p.UEventId = uPayment.UEventId;
                p.UMemberId = uPayment.UMemberId;
                p.Id = uPayment.Id;

                p.MemberName = _context.UMembers.FirstOrDefault(x => x.Id == p.UMemberId).Name;
            }
            return uPayments;
        }

        // GET: api/UPayments
        [HttpGet]
        public IEnumerable<UPayment> GetUPayments()
        {
            return _context.UPayments;
        }

        // GET: api/UPayments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUPayment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UPayment uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);

            if (uPayment == null)
            {
                return NotFound();
            }

            return Ok(uPayment);
        }

        // PUT: api/UPayments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUPayment([FromRoute] int id, [FromBody] UPayment uPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uPayment.Id)
            {
                return BadRequest();
            }

            _context.Entry(uPayment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UPaymentExists(id))
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

        // POST: api/UPayments
        [HttpPost]
        public async Task<IActionResult> PostUPayment([FromBody] UPayment uPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UPayments.Add(uPayment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UPaymentExists(uPayment.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUPayment", new { id = uPayment.Id }, uPayment);
        }

        // DELETE: api/UPayments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUPayment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UPayment uPayment = await _context.UPayments.SingleOrDefaultAsync(m => m.Id == id);
            if (uPayment == null)
            {
                return NotFound();
            }

            _context.UPayments.Remove(uPayment);
            await _context.SaveChangesAsync();

            return Ok(uPayment);
        }

        private bool UPaymentExists(int id)
        {
            return _context.UPayments.Any(e => e.Id == id);
        }
    }
}