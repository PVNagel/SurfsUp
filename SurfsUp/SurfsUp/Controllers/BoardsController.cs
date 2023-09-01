using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public async Task<IActionResult> Index(string searchString, string selectedProperty)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
            }
            var modelProperties = typeof(Board).GetProperties(); 
            ViewBag.PropertyList = new SelectList(modelProperties, "Name", "Name");
            
            ViewData["CurrentFilter"] = searchString;

            var boards = from b in _context.Board
                         select b;

            if (selectedProperty != null)
            {
                var searchBoards = new List<Board>();
                foreach (Board board in boards)
                {
                    PropertyInfo propertyInfo = board.GetType().GetProperty(selectedProperty);
                    if(propertyInfo != null)
                    {
                        object propertyValue = propertyInfo.GetValue(board);
                        if (propertyValue != null)
                        {
                            if (!String.IsNullOrEmpty(searchString))
                            {
                                if (propertyValue.ToString().ToLower().Contains(searchString))
                                {
                                    searchBoards.Add(board);
                                }
                            }
                            else
                            {
                                searchBoards.Add(board);
                            }
                        }
                    }
                }

                return searchBoards != null ?
            View(searchBoards) :
            Problem("Entity set 'SurfsUpContext.Board'  is null.");
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                var searchBoards = boards.ToList();
                var result = searchBoards.Where(b => b.Name.ToLower().Contains(searchString) ||
                                           b.Length.Contains(searchString) ||
                                           b.Width.Contains(searchString) ||
                                           b.Thickness.Contains(searchString) ||
                                           b.Volume.Contains(searchString) ||
                                           b.Type.ToString().ToLower().Contains(searchString) ||
                                           String.IsNullOrEmpty(b.Equipment) == false && b.Equipment.ToLower().Contains(searchString) ||
                                           b.Price.ToString().Contains(searchString));

                return View(result);
            }

            return
                        View(await boards.AsNoTracking().ToListAsync());

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
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path);

                List<IFormFile> files = new List<IFormFile>();  // List that will hold the files and subfiles in path
                foreach (string filePath in filePaths)
                {
                    using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        IFormFile file = new FormFile(
                        baseStream: stream,
                        baseStreamOffset: 0,
                        length: new System.IO.FileInfo(filePath).Length,
                        name: "formFile",
                        fileName: System.IO.Path.GetFileName(filePath));
                        files.Add(file);
                    }
                }

                board.Attachments = files;
            }
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

                if(board.Attachments != null)
                {
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

            string rootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(rootPath + "/Images/" + id);
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path);

                List<IFormFile> files = new List<IFormFile>();  // List that will hold the files and subfiles in path
                foreach (string filePath in filePaths)
                {
                    using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        IFormFile file = new FormFile(
                        baseStream: stream,
                        baseStreamOffset: 0,
                        length: new System.IO.FileInfo(filePath).Length,
                        name: "formFile",
                        fileName: System.IO.Path.GetFileName(filePath));
                        files.Add(file);
                    }
                }

                board.Attachments = files;
            }




            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Price,Equipment,Attachments")] Board board)
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

                    if(board.Attachments != null)
                    {
                        string rootPath = _webHostEnvironment.WebRootPath;
                        foreach (var formFile in board.Attachments)
                        {
                            if (formFile.Length > 0)
                            {
                                var filePath = Path.Combine(rootPath + "/Images/" + id);

                                if (!Directory.Exists(filePath))
                                {
                                    Directory.CreateDirectory(filePath);
                                }

                                filePath = Path.Combine(rootPath + "/Images/" + id, formFile.FileName);

                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                            }
                        }
                    }
                    

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

            string rootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(rootPath + "/Images/" + id);
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path);

                List<IFormFile> files = new List<IFormFile>();  // List that will hold the files and subfiles in path
                foreach (string filePath in filePaths)
                {
                    using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        IFormFile file = new FormFile(
                        baseStream: stream,
                        baseStreamOffset: 0,
                        length: new System.IO.FileInfo(filePath).Length,
                        name: "formFile",
                        fileName: System.IO.Path.GetFileName(filePath));
                        files.Add(file);
                    }
                }

                board.Attachments = files;
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

            string rootPath = _webHostEnvironment.WebRootPath;


            var filePath = Path.Combine(rootPath + "/Images/" + id);

            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
          return (_context.Board?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //POST: Boards/Edit/DeleteImg
        [HttpPost, ActionName("DeleteImgConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImgConfirmed(string fileName, int id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var filePath = Path.Combine(rootPath + "/Images/" + id, fileName);

            System.IO.File.Delete(filePath);

            return RedirectToAction(nameof(Edit), new { id = id });
        }
    }
}
