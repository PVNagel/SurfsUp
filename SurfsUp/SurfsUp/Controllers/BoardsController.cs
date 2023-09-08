using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string selectedProperty, int? pageNumber)
        {
            int pageSize = 5;
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
            }
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_Desc" : "Length";
            ViewData["WidthSortParm"] = sortOrder == "Width" ? "Width_Desc" : "Width";
            ViewData["ThicknessSortParm"] = sortOrder == "Thickness" ? "Thickness_Desc" : "Thickness";
            ViewData["VolumeSortParm"] = sortOrder == "Volume" ? "Volume_Desc" : "Volume";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "Type_Desc" : "Type";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "Price_Desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var modelProperties = typeof(Board).GetProperties(); 
            ViewBag.PropertyList = new SelectList(modelProperties, "Name", "Name");
            
            ViewData["CurrentFilter"] = searchString;

            var boards = from b in _context.Board select b;


            switch (sortOrder)
            {
                case "Name_Desc":
                    boards = boards.OrderByDescending(s => s.Name);
                    break;
                case "Length":
                    boards = boards.OrderBy(s => s.Length);
                    break;
                case "Length_Desc":
                    boards = boards.OrderByDescending(s => s.Length);
                    break;
                case "Width":
                    boards = boards.OrderBy(s => s.Width);
                    break;
                case "Width_Desc":
                    boards = boards.OrderByDescending(s => s.Width);
                    break;
                case "Thickness":
                    boards = boards.OrderBy(s => s.Thickness);
                    break;
                case "Thickness_Desc":
                    boards = boards.OrderByDescending(s => s.Thickness);
                    break;
                case "Volume":
                    boards = boards.OrderBy(s => s.Volume);
                    break;
                case "Volume_Desc":
                    boards = boards.OrderByDescending(s => s.Volume);
                    break;
                case "Type":
                    boards = boards.OrderBy(s => s.Type);
                    break;
                case "Type_Desc":
                    boards = boards.OrderByDescending(s => s.Type);
                    break;
                case "Price":
                    boards = boards.OrderBy(s => s.Price);
                    break;
                case "Price_Desc":
                    boards = boards.OrderByDescending(s => s.Price);
                    break;
                default:
                    boards = boards.OrderBy(s => s.Name);
                    break;
            }

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

                var paginatedList = await PaginatedList<Board>.CreateAsync(searchBoards, pageNumber ?? 1, pageSize);
                return View(paginatedList);
            }
            
            if (!String.IsNullOrEmpty(searchString))
            {
                var searchBoards = boards.ToList();
                var result = searchBoards.Where(b => b.Name.ToLower().Contains(searchString) ||
                                           b.Length.ToString().Contains(searchString) ||
                                           b.Width.ToString().Contains(searchString) ||
                                           b.Thickness.ToString().Contains(searchString) ||
                                           b.Volume.ToString().Contains(searchString) ||
                                           b.Type.ToString().ToLower().Contains(searchString) ||
                                           String.IsNullOrEmpty(b.Equipment) == false && b.Equipment.ToLower().Contains(searchString) ||
                                           b.Price.ToString().Contains(searchString));

                var paginatedList = await PaginatedList<Board>.CreateAsync(result.ToList(), pageNumber ?? 1, pageSize);
                return View(paginatedList);
            }


            return View(await PaginatedList<Board>.CreateAsync(await boards.AsNoTracking().ToListAsync(), pageNumber ?? 1, pageSize));
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Board());
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteImgConfirmed(string fileName, int id)
        {
            string rootPath = _webHostEnvironment.WebRootPath;

            var filePath = Path.Combine(rootPath + "/Images/" + id, fileName);

            System.IO.File.Delete(filePath);

            return RedirectToAction(nameof(Edit), new { id = id });
        }
    }
}
