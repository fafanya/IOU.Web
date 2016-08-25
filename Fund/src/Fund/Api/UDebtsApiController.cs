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
    [Route("api/UDebtsApi")]
    public class UDebtsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UDebtsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ByGroup/{id}")]
        public IEnumerable<UDebt> ByGroup([FromRoute] int id)
        {
            IEnumerable<UDebt> uDebts = _context.UDebts.Where(x => x.UGroupId == id);

            List<UDebt> result = new List<UDebt>();
            foreach(UDebt uDebt in uDebts)
            {
                UDebt d = new UDebt();
                d.Amount = uDebt.Amount;
                d.DebtorId = uDebt.DebtorId;
                d.LenderId = uDebt.LenderId;
                d.Name = uDebt.Name;
                d.UGroupId = uDebt.UGroupId;
                d.Id = uDebt.Id;

                d.LenderName = _context.UMembers.FirstOrDefault(x => x.Id == d.LenderId).Name;
                d.DebtorName = _context.UMembers.FirstOrDefault(x => x.Id == d.DebtorId).Name;

                result.Add(d);
            }
            return result;
        }

        // GET: api/UDebts
        [HttpGet]
        public IEnumerable<UDebt> GetUDebts()
        {
            return _context.UDebts;
        }

        // GET: api/UDebts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUDebt([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UDebt uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);

            if (uDebt == null)
            {
                return NotFound();
            }

            return Ok(uDebt);
        }

        // PUT: api/UDebts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUDebt([FromRoute] int id, [FromBody] UDebt uDebt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uDebt.Id)
            {
                return BadRequest();
            }

            _context.Entry(uDebt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UDebtExists(id))
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

        // POST: api/UDebts
        [HttpPost]
        public async Task<IActionResult> PostUDebt([FromBody] UDebt uDebt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UDebts.Add(uDebt);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UDebtExists(uDebt.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUDebt", new { id = uDebt.Id }, uDebt);
        }

        // DELETE: api/UDebts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUDebt([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UDebt uDebt = await _context.UDebts.SingleOrDefaultAsync(m => m.Id == id);
            if (uDebt == null)
            {
                return NotFound();
            }

            _context.UDebts.Remove(uDebt);
            await _context.SaveChangesAsync();

            return Ok(uDebt);
        }

        private bool UDebtExists(int id)
        {
            return _context.UDebts.Any(e => e.Id == id);
        }
    }
}