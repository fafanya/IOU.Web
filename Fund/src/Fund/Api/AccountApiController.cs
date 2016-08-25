using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fund.Data;
using Fund.Models;
using Fund.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;

namespace Fund.Api
{
    [Produces("application/json")]
    [Route("api/AccountApi")]
    public class AccountApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UUser> _userManager;
        private readonly SignInManager<UUser> _signInManager;

        public AccountApiController(ApplicationDbContext context, UserManager<UUser> userManager,
            SignInManager<UUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Account
        [HttpGet]
        public IEnumerable<UUser> GetUUsers()
        {
            return _context.UUsers;
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UUser uUser = await _context.UUsers.SingleOrDefaultAsync(m => m.Id == id);

            if (uUser == null)
            {
                return NotFound();
            }

            return Ok(uUser);
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUUser([FromRoute] string id, [FromBody] UUser uUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != uUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(uUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UUserExists(id))
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var uUser = new UUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(uUser, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new { Id = uUser.Id, Email = uUser.Email });
                }
            }
            return Ok(false);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    UUser uUser = _context.UUsers.SingleOrDefault(x => x.Email == model.Email);
                    if (uUser != null)
                    {
                        //return Ok( new { Id = uUser.Id, Email = uUser.Email });
                        return Json(new { Id = uUser.Id, Email = uUser.Email });
                    }
                }
            }

            return Ok(false);
        }

        // POST: api/Account
        [HttpPost]
        public async Task<IActionResult> PostUUser([FromBody] UUser uUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UUsers.Add(uUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UUserExists(uUser.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUUser", new { id = uUser.Id }, uUser);
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UUser uUser = await _context.UUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (uUser == null)
            {
                return NotFound();
            }

            _context.UUsers.Remove(uUser);
            await _context.SaveChangesAsync();

            return Ok(uUser);
        }

        private bool UUserExists(string id)
        {
            return _context.UUsers.Any(e => e.Id == id);
        }
    }
}