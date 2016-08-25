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
    [Route("api/UEventsApi")]
    public class UEventsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UEventsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ByGroup/{id}")]
        public IEnumerable<UEvent> ByGroup([FromRoute] int id)
        {
            IEnumerable<UEvent> uEvents = _context.UEvents.Where(x => x.UGroupId == id);

            List<UEvent> result = new List<UEvent>();
            foreach (UEvent uEvent in uEvents)
            {
                UEvent e = new UEvent();
                e.Name = uEvent.Name;
                e.Id = uEvent.Id;
                e.UGroupId = uEvent.UGroupId;
                e.UEventTypeId = uEvent.UEventTypeId;

                e.EventTypeName = _context.UEventTypes.FirstOrDefault(x => x.Id == e.UEventTypeId).Name;

                result.Add(e);
            }
            return result;
        }

        // GET: api/UEvents
        [HttpGet]
        public IEnumerable<UEvent> GetUEvents()
        {
            return _context.UEvents;
        }

        // GET: api/UEvents/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UEvent uEvent = await _context.UEvents.SingleOrDefaultAsync(m => m.Id == id);

            if (uEvent == null)
            {
                return NotFound();
            }

            return Ok(uEvent);
        }

        // PUT: api/UEvents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUEvent([FromRoute] int id, [FromBody] UEvent uEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uEvent.Id)
            {
                return BadRequest();
            }

            _context.Entry(uEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UEventExists(id))
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

        // POST: api/UEvents
        [HttpPost]
        public async Task<IActionResult> PostUEvent([FromBody] UEvent uEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UEvents.Add(uEvent);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UEventExists(uEvent.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUEvent", new { id = uEvent.Id }, uEvent);
        }

        // DELETE: api/UEvents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UEvent uEvent = await _context.UEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (uEvent == null)
            {
                return NotFound();
            }

            var payments = _context.UPayments.Where(x => x.UEventId == uEvent.Id).ToList();
            _context.RemoveRange(payments);
            var bills = _context.UBills.Where(x => x.UEventId == uEvent.Id).ToList();
            _context.RemoveRange(bills);

            _context.UEvents.Remove(uEvent);
            await _context.SaveChangesAsync();

            return Ok(uEvent);
        }

        private bool UEventExists(int id)
        {
            return _context.UEvents.Any(e => e.Id == id);
        }
    }
}