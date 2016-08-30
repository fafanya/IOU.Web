using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;
using Fund.Models.UMemberViewModels;

namespace Fund.Api
{
    [Produces("application/json")]
    [Route("api/UMembersApi")]
    public class UMembersApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UMembersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Select")]
        public IActionResult Select([FromBody] SelectViewModel select)
        {
            UMember currentMember = _context.UMembers.SingleOrDefault(x => x.Id == select.UMemberId);
            if (currentMember != null)
            {
                currentMember.UUserId = select.UUserId;
                _context.Update(currentMember);

                UMember previousMember = _context.UMembers.SingleOrDefault(x => x.UUserId == select.UUserId
                && x.UGroupId == currentMember.UGroupId);
                if (previousMember != null)
                {
                    previousMember.UUserId = null;
                    _context.Update(previousMember);
                }

                _context.SaveChanges(true);
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet("ByGroup/{id}")]
        public IEnumerable<UMember> ByGroup([FromRoute] int id)
        {
            IEnumerable<UMember> uMembers = _context.UMembers.Where(x => x.UGroupId == id);
            return uMembers;
        }

        // GET: api/UMembers
        [HttpGet]
        public IEnumerable<UMember> GetUMembers()
        {
            return _context.UMembers;
        }

        // GET: api/UMembers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUMember([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UMember uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);

            if (uMember == null)
            {
                return NotFound();
            }

            return Ok(uMember);
        }

        // PUT: api/UMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUMember([FromRoute] int id, [FromBody] UMember uMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uMember.Id)
            {
                return BadRequest();
            }

            _context.Entry(uMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UMemberExists(id))
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

        // POST: api/UMembers
        [HttpPost]
        public async Task<IActionResult> PostUMember([FromBody] UMember uMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UMembers.Add(uMember);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UMemberExists(uMember.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUMember", new { id = uMember.Id }, uMember);
        }

        // DELETE: api/UMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUMember([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UMember uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);
            if (uMember == null)
            {
                return NotFound();
            }

            _context.UMembers.Remove(uMember);
            await _context.SaveChangesAsync();

            return Ok(uMember);
        }

        private bool UMemberExists(int id)
        {
            return _context.UMembers.Any(e => e.Id == id);
        }
    }
}