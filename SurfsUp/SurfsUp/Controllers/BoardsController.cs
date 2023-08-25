using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Data;
using SurfsUp.Models;

namespace SurfsUp.Controllers
{
    public class BoardsController : Controller
    {
        private readonly SurfsUpContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BoardsController(SurfsUpContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Boards
        public async Task<IActionResult> Index()
        {
              return _context.Board != null ? 
                          View(await _context.Board.ToListAsync()) :
                          Problem("Entity set 'SurfsUpContext.Board'  is null.");
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            string rootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(rootPath + "/Images/" + id);
            string[] filePaths = Directory.GetFiles(path);

            List<IFormFile> files = new List<IFormFile>();  // List that will hold the files and subfiles in path
            foreach (string filePath in filePaths)
            {
                IFormFile file = new FormFile(
                    baseStream: new System.IO.FileStream(filePath, System.IO.FileMode.Open),
                    baseStreamOffset: 0,
                    length: new System.IO.FileInfo(filePath).Length,
                    name: "formFile",
                    fileName: System.IO.Path.GetFileName(filePath));
                files.Add(file);
            }

            board.Attachments = files;
            return View(board);
        }

        // GET: Boards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Length,Width,Thickness,Volume,Price,Equipment,Attachments")] Board board)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Add(board).Entity;
                await _context.SaveChangesAsync();

                long size = board.Attachments.Sum(f => f.Length);
                string rootPath = _webHostEnvironment.WebRootPath;
                foreach (var formFile in board.Attachments)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(rootPath + "/Images/" + entity.Id);

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        filePath = Path.Combine(rootPath + "/Images/" + entity.Id, formFile.FileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Price,Equipment")] Board board)
        {
            if (id != board.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Id))
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
            return View(board);
        }

        // GET: Boards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Board == null)
            {
                return Problem("Entity set 'SurfsUpContext.Board'  is null.");
            }
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
          return (_context.Board?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
