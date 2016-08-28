using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;
using Fund.Models.UMemberViewModels;

namespace Fund.Controllers
{
    public class UMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UMembersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: UMembers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UMembers.Include(u => u.UUser).Include(u => u.UGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);
            if (uMember == null)
            {
                return NotFound();
            }

            return View(uMember);
        }

        // GET: UMembers/Create
        public IActionResult Create(int? uGroupId)
        {
            ViewData["UGroupId"] = uGroupId;
            return View();
        }

        // POST: UMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UGroupId")] UMember uMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uMember);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "UGroups", new { id = uMember.UGroupId });
            }
            ViewData["UUserId"] = new SelectList(_context.UUsers, "Id", "Id", uMember.UUserId);
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uMember.UGroupId);
            return View(uMember);
        }

        // GET: UMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);
            if (uMember == null)
            {
                return NotFound();
            }
            return View(uMember);
        }

        // POST: UMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name, Id, UUserId, UGroupId")] UMember uMember)
        {
            if (id != uMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UMemberExists(uMember.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "UGroups", new { id = uMember.UGroupId });
            }
            ViewData["UUserId"] = new SelectList(_context.UUsers, "Id", "Id", uMember.UUserId);
            ViewData["UGroupId"] = new SelectList(_context.UGroups, "Id", "Id", uMember.UGroupId);
            return View(uMember);
        }

        // GET: UMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);
            if (uMember == null)
            {
                return NotFound();
            }

            return View(uMember);
        }

        // POST: UMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uMember = await _context.UMembers.SingleOrDefaultAsync(m => m.Id == id);
            _context.UMembers.Remove(uMember);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "UGroups", new { id = uMember.UGroupId });
        }

        private bool UMemberExists(int id)
        {
            return _context.UMembers.Any(e => e.Id == id);
        }

        public IActionResult Select(int? id)
        {
            ViewData["UMemberId"] = new SelectList(_context.UMembers.Where(x => x.UGroupId == id
            && x.UUserId == null), "Id", "Name");
            ViewData["UUserId"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }
        [HttpPost]
        public IActionResult Select([Bind("UMemberId, UUserId")] SelectViewModel select)
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
                return RedirectToAction("Details", "UGroups", new { id = currentMember.UGroupId });
            }

            return RedirectToAction("Index", "UGroups");
        }
    }
}
