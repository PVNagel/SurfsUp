using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Data;
using SurfsUp.Models;
using SurfsUp.Services;

namespace SurfsUp.Controllers
{
    [Authorize]
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Renting> rentings = User.IsInRole("Admin") ?
                await _context.Renting.Include(r => r.Board).Include(r => r.SurfsUpUser).ToListAsync() :
                await _context.Renting.Include(r => r.Board).Include(r => r.SurfsUpUser).Where(x => x.SurfsUpUserId == userId).ToListAsync();

            return View(rentings);
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
        public async Task<IActionResult> Create(int boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            RentingQueueService.AddPosition(new RentingQueuePosition()
            {
                SurfsUpUserId = userId,
                QueueJoined = DateTime.Now,
                BoardId = boardId
            });
            var renting = new Renting { BoardId = boardId, SurfsUpUserId = userId, EndDate = DateTime.Now };
            ViewData["BoardName"] = board.Name;
            return View(renting);
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
                    renting.StartDate = DateTime.Now;
                    var rentings = _context.Renting.Where(x => x.BoardId == renting.BoardId);
                    foreach(var item in rentings)
                    {
                        // hvis nye rentings startdate er før item enddate
                        // ELLER renting startdate er efter item startdate og renting enddate er før item enddate 
                        if (renting.StartDate < item.EndDate || renting.StartDate > item.StartDate && renting.EndDate < item.EndDate)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Unable to create new renting because another renting has already been created.");
                            return View(renting);
                        }
                    }

                    if (!RentingQueueService.IsFirstPosition(RentingQueueService.GetPosition(renting.SurfsUpUserId)))
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to create new renting because another user is currently infront of you in the queue");
                        return View(renting);
                    }

                    if (!RentingQueueService.RemovePosition(renting.SurfsUpUserId))
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to remove you from the renting queue, please try again.");
                        return View(renting);
                    }

                    _context.Add(renting);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
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
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", renting.BoardId);
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
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", renting.BoardId);
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

        //Bliver kaldt fra userLeavesRentingPage.js i /renting/create view, hvis man går væk fra siden uden at trykke create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQueuePosition()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (RentingQueueService.RemovePosition(userId))
            {
                return Ok(new { Message = "Anmodningen blev behandlet med succes." });
            }
            return BadRequest(new { Message = "Anmodningen kunne ikke fuldføres."});
        }

        private bool RentingExists(int id)
        {
            return (_context.Renting?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
