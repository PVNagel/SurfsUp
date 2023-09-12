using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Data;
using SurfsUp.Models;
using SurfsUp.Services;

namespace SurfsUp.Controllers
{
    public class BoardsController : Controller
    {
        private readonly SurfsUpContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<SurfsUpUser> _userManager;
        private readonly ImageService _imageService;

        public BoardsController(SurfsUpContext context, IWebHostEnvironment webHostEnvironment, UserManager<SurfsUpUser> userManager, ImageService imageService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            this._userManager = userManager;
            _imageService = imageService;
        }

        // GET: Boards
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string selectedProperty, int? pageNumber)
        {
            int pageSize = 5;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_Desc" : "Length";
            ViewData["WidthSortParm"] = sortOrder == "Width" ? "Width_Desc" : "Width";
            ViewData["ThicknessSortParm"] = sortOrder == "Thickness" ? "Thickness_Desc" : "Thickness";
            ViewData["VolumeSortParm"] = sortOrder == "Volume" ? "Volume_Desc" : "Volume";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "Type_Desc" : "Type";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "Price_Desc" : "Price";

            var modelProperties = typeof(Board).GetProperties(); 
            ViewBag.PropertyList = new SelectList(modelProperties, "Name", "Name");
            
            ViewData["CurrentFilter"] = searchString;

            string userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            var boardsList = _context.Boards.Include(x => x.Rentings).ToList();
            var boards = RemoveBoardsRentedByOthers(boardsList, userId);

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
                var searchBoards = MakeNewListFilteredByProperty(boards, selectedProperty, searchString);
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

            return View(await PaginatedList<Board>.CreateAsync(boards.AsNoTracking().ToList(), pageNumber ?? 1, pageSize));
        }

        
        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Boards == null)
            {
                return NotFound();
            }

            var board = await _context.Boards
                .Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Create([Bind("Name,Length,Width,Thickness,Volume,Type,Price,Equipment")] Board board, IList<IFormFile>? attachments)
        {
            if (ModelState.IsValid)
            {

                var entity = _context.Add(board).Entity;
                await _context.SaveChangesAsync();

                if (attachments != null)
                {
                    await _imageService.SaveImages(entity.Id, attachments);
                }
              
                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }


        // GET: Boards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Boards == null)
            {
                return NotFound();
            }

            var board = await _context.Boards
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment")] Board board, IList<IFormFile>? attachments)
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

                    if (attachments != null)
                    {
                        await _imageService.SaveImages(id, attachments);
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
            if (id == null || _context.Boards == null)
            {
                return NotFound();
            }

            var board = await _context.Boards
                .Include(x => x.Images)
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Boards == null)
            {
                return Problem("Entity set 'SurfsUpContext.Board'  is null.");
            }
            var board = await _context.Boards.FindAsync(id);
            if (board != null)
            {
                await _imageService.DeleteImagesAsync(id);
                _context.Boards.Remove(board);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
            return (_context.Boards?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static IQueryable<Board> RemoveBoardsRentedByOthers(List<Board> boards, string userId)
        {
            // Opret en liste til at holde de boards, der skal fjernes
            var boardsToRemove = new List<Board>();

            foreach (var board in boards)
            {
                foreach (var renting in board.Rentings)
                {
                    if (renting.SurfsUpUserId != userId || userId == null)
                    {
                        if (DateTime.Now > renting.StartDate && DateTime.Now < renting.EndDate)
                        {
                            // Tilføj dette board til listen over boards, der skal fjernes
                            boardsToRemove.Add(board);
                            break; // Du kan bryde ud af den indre løkke, når en leasing er fundet
                        }
                    }
                }
            }

            // Fjern de boards, der er markeret til fjernelse
            foreach (var boardToRemove in boardsToRemove)
            {
                boards.Remove(boardToRemove);
            }

            return boards.AsQueryable();
        }

        private static List<Board> MakeNewListFilteredByProperty(IQueryable<Board> boards, string selectedProperty, string searchString)
        {
            var searchBoards = new List<Board>();
            foreach (Board board in boards)
            {
                PropertyInfo propertyInfo = board.GetType().GetProperty(selectedProperty);
                if (propertyInfo != null)
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

            return searchBoards;
        }
    }
}
