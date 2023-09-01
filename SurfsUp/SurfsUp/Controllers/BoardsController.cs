using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "Price_Desc" : "Price";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_Desc" : "Length";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "Type_Desc" : "Type";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var boards = from s in _context.Board select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                boards = boards.Where(s => s.Name.Contains(searchString)
                                       || s.Length.Contains(searchString));
            }
           

            //return _context.Board != null ? 
            //          View(await _context.Board.ToListAsync()) :
            //          Problem("Entity set 'SurfsUpContext.Board'  is null.");

            switch (sortOrder)
            {
                case "Name_Desc":
                    boards = boards.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    boards = boards.OrderBy(s => s.Price);
                    break;
                case "Price_desc":
                    boards = boards.OrderByDescending(s => s.Price);
                    break;
                case "Length":
                    boards = boards.OrderBy(s => s.Length);
                    break;
                case "Length_desc":
                    boards = boards.OrderByDescending(s => s.Length);
                    break;
                case "Type":
                    boards = boards.OrderBy(s => s.Type);
                    break;
                case "Type_desc":
                    boards = boards.OrderByDescending(s => s.Type);
                    break;
                default:
                    boards = boards.OrderBy(s => s.Name);
                    break;

            }

            int pageSize = 5;

            return View(await PaginatedList<Board>.CreateAsync(boards.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await boards.AsNoTracking().ToListAsync());
                        

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
        public async Task<IActionResult> Create([Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,Attachments")] Board board)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,Attachments")] Board board)
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
