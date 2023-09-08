using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Data;
using SurfsUp.Models;

namespace SurfsUp.Controllers
{
    public class RentingsController : Controller
    {
        private readonly SurfsUpContext _context;
        private readonly UserManager<SurfsUpUser> _userManager;

        public RentingsController(SurfsUpContext context, UserManager<SurfsUpUser> _userManager)
        {
            _userManager = _userManager;
            _context = context;
        }

        // GET: Rentings
        public async Task<IActionResult> Index()
        {
            var surfsUpContext = _context.Renting.Include(r => r.Board).Include(r => r.SurfsUpUser);
            return View(await surfsUpContext.ToListAsync());
        }

        // GET: Rentings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Renting == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting
                .Include(r => r.Board)
                .Include(r => r.SurfsUpUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // GET: Rentings/Create
        public IActionResult Create(int boardId)
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = nameIdentifierClaim.Value;
            ViewData["UserId"] = userId;
            ViewData["BoardId"] = boardId;
            return View();
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardId,StartDate,EndDate,SurfsUpUserId")] Renting renting)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(renting);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", renting.BoardId);
                ViewData["SurfsUpUserId"] = new SelectList(_context.Users, "Id", "Id", renting.SurfsUpUserId);
                return View(renting);



            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Rentings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Renting == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting.FindAsync(id);
            if (renting == null)
            {
                return NotFound();
            }
            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", renting.BoardId);
            ViewData["SurfsUpUserId"] = new SelectList(_context.Users, "Id", "Id", renting.SurfsUpUserId);
            return View(renting);
        }

        // POST: Rentings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SurfsUpUserId,BoardId,StartDate,EndDate")] Renting renting)
        {
            if (id != renting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(renting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentingExists(renting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", renting.BoardId);
            ViewData["SurfsUpUserId"] = new SelectList(_context.Users, "Id", "Id", renting.SurfsUpUserId);
            return View(renting);
        }

        // GET: Rentings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Renting == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting
                .Include(r => r.Board)
                .Include(r => r.SurfsUpUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // POST: Rentings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Renting == null)
            {
                return Problem("Entity set 'SurfsUpContext.Renting'  is null.");
            }
            var renting = await _context.Renting.FindAsync(id);
            if (renting != null)
            {
                _context.Renting.Remove(renting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentingExists(int id)
        {
            return (_context.Renting?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
