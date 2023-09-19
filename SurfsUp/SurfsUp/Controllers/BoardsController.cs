using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer.Localisation;
using MessagePack;
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
        // her laver vi fields som vi sætter til vores services. 
        private readonly SurfsUpContext _context; //dbcontext er en scoped service
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageService _imageService; // en scoped service som vi har initialized i Program.cs. Den håndtere images

        public BoardsController(
            //her bruger vi dependency injection til at hente vores services ind i controlleren igennem constructoren.
            SurfsUpContext context,  
            IWebHostEnvironment webHostEnvironment, 
            ImageService imageService) 
        {
            // sætter vores fields til vores injectede services
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            
            // Sortering
            // Nogen der kan give en lækker forklaring på det her?
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_Desc" : "Length";
            ViewData["WidthSortParm"] = sortOrder == "Width" ? "Width_Desc" : "Width";
            ViewData["ThicknessSortParm"] = sortOrder == "Thickness" ? "Thickness_Desc" : "Thickness";
            ViewData["VolumeSortParm"] = sortOrder == "Volume" ? "Volume_Desc" : "Volume";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "Type_Desc" : "Type";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "Price_Desc" : "Price";

            // Sætter vores Board Properties i en Selectlist,
            // som er en liste vi kan bruge i vores select html tag så det bliver dynamisk sat ind.
            var modelProperties = typeof(Board).GetProperties();
            ViewBag.PropertyList = new SelectList(modelProperties, "Name", "Name");
            
            ViewData["CurrentFilter"] = searchString;

            if(selectedProperty != null)
            {
                ViewData["selectedProperty"] = selectedProperty;
            }

            // User er et ClaimsPrincipal objekt, som indeholder information om den nuværende bruger.
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {
                // Vi henter userId, som vi skal bruge til at fjerne de boards som er lejet af andre brugere.
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            var boardsList = _context.Boards.Include(x => x.Rentings).ToList();
            // RemoveBoardsRentedByOthers er en metode som vi har lavet til at gøre koden mere læselig.
            // Jeg har bare gjort det så metoden ikke var så lang og uoverskuelig. (Den findes i bunden af controlleren)
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

            // selectedProperty er den parameter vi bruger hvis man vælger at søge specifikt efter bestemte properties.
            if (selectedProperty != null)
            {
                // MakeNewListFilteredByProperty er en metode vi har lavet til at gøre koden mere læselig. 
                // Jeg har bare gjort det så metoden ikke var så lang og uoverskuelig. (Den findes i bunden af controlleren)
                var searchBoards = MakeNewListFilteredByProperty(boards, selectedProperty, searchString);

                // PaginatedList er vores egen klasse.
                // CreateAsync laver en ny paginatedList med de parametre vi giver.
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
                                                                                                                            // attachments er ikke længere en del af board,
                                                                                                                            // da det gav mere mening at de er adskilt og board kun har en 
                                                                                                                            // Property der hedder Images
        public async Task<IActionResult> Create([Bind("Name,Length,Width,Thickness,Volume,Type,Price,Equipment")] Board board, IList<IFormFile>? attachments)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Add(board).Entity;
                await _context.SaveChangesAsync();

                if (attachments != null)
                {
                    // _imageService er en service vi har lavet som håndtere alt med images.
                    // SaveImages Gemmer et image objekt i databasen som har en relation til et
                    // board via boardId, samt et image path. billedfilerne gemmes i wwwroot
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
                .AsNoTracking()
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
        [Authorize(Roles = "Admin")]                                                                                                    // attachments er ikke længere en del af board,
                                                                                                                                        // da det gav mere mening at de er adskilt og board kun har en 
                                                                                                                                        // Property der hedder Images
        public async Task<IActionResult> Edit(int id, IList<IFormFile>? attachments, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardToUpdate = await _context.Boards.FirstOrDefaultAsync(b => b.Id == id);
            // Denne funktion tjekker om et board er blevet deleted, imens en anden er igang med at ændre det
            if (boardToUpdate == null)
            {
                Board deletedBoard = new Board();
                await TryUpdateModelAsync(deletedBoard);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The board was deleted by another user.");
                return View(deletedBoard);
            }

            _context.Entry(boardToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            // Denne lange if sætning tjekker om nogle af boardes properties er blevet ændret i edit.
            // Den thrower en exception hvis en prøver at save, når en anden bruger har gjort det først.
            if (await TryUpdateModelAsync<Board>(
               boardToUpdate,
               "",
               b => b.Name, b => b.Length, b => b.Width, b => b.Thickness, b => b.Volume, b => b.Type, b => b.Price, b => b.Equipment, b => b.Images))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Board)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    
                        var databaseValues = (Board)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Length != clientValues.Length)
                        {
                            ModelState.AddModelError("Length", $"Current value: {databaseValues.Length:c}");
                        }
                        if (databaseValues.Width != clientValues.Width)
                        {
                            ModelState.AddModelError("Width", $"Current value: {databaseValues.Width:d}");
                        }
                        if (databaseValues.Thickness != clientValues.Thickness)
                        {
                            ModelState.AddModelError("Thickness", $"Current value: {databaseValues.Thickness:d}");

                        }
                        if (databaseValues.Volume != clientValues.Volume)
                        {
                            ModelState.AddModelError("Volume", $"Current value: {databaseValues.Volume:d}");

                        }
                        if (databaseValues.Type != clientValues.Type)
                        {
                            ModelState.AddModelError("Type", $"Current value: {databaseValues.Type:d}");

                        }
                        if (databaseValues.Price != clientValues.Price)
                        {
                            ModelState.AddModelError("Price", $"Current value: {databaseValues.Price:d}");

                        }
                        if (databaseValues.Equipment != clientValues.Equipment)
                        {
                            ModelState.AddModelError("Equipment", $"Current value: {databaseValues.Equipment:d}");
                        }
                        if (databaseValues.Images != clientValues.Images)
                        {
                            ModelState.AddModelError("Images", $"Current value: {databaseValues.Images:d}");
                        }
                        ModelState.AddModelError(string.Empty, "The board you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        boardToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(id);
                    await _context.SaveChangesAsync();

                    if (attachments != null)
                    {
                        // _imageService er en service vi har lavet som håndtere alt med images.
                        // SaveImages Gemmer et image objekt i databasen som har en relation til et
                        // board via boardId, samt et image path. billedfilerne gemmes i wwwroot
                        await _imageService.SaveImages(id, attachments);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(id))
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

          

           
            return View(boardToUpdate);
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
                // DeleteImagesAsync sletter alle images med boardId i databasen
                // og sletter alle billedfilerne i wwwroot/images som tilhørte de images
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

        // Fjerner de boards fra listen boards, som allerede er lejet af en anden. 
        // og returnere listen som IQueryable<Boards>
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

        // Hvis en bestemt property(selectedProperty) er valgt ved søgefeltet, 
        // laver MakeNewListFilteredByProperty en ny liste hvor den filtrerer 
        // så det kun er boards med den property der kommer med, derefter 
        // yderligere filtrering med searchString hvis der også er skrevet noget i den.
        // Returnerer en liste af boards
        private static List<Board> MakeNewListFilteredByProperty(IQueryable<Board> boards, string selectedProperty, string searchString)
        {
            var searchBoards = new List<Board>();
            // tjekker alle boards
            foreach (Board board in boards)
            {
                // board.GetType() kunne også være typeof(Board) får et Type objekt af Board,
                // som vi kan kalde GetProperty(selectedProperty) på som får propertyInfo for den selectedProperty 
                // som er valgt af brugeren.
                PropertyInfo propertyInfo = board.GetType().GetProperty(selectedProperty);
                if (propertyInfo != null)
                {
                    // får den specifikke værdi af propertien som blev valgt for det board vi tjekker.
                    object propertyValue = propertyInfo.GetValue(board);

                    // ser om boardet har en værdi for den Property
                    if (propertyValue != null)
                    {
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            // hvis brugeren også har skrevet noget i searchstring, 
                            // tjekkes om property værdien indeholder searchstring
                            if (propertyValue.ToString().ToLower().Contains(searchString))
                            {
                                // hvis den gør, tilføjes boardet.
                                searchBoards.Add(board);
                            }
                        }
                        // hvis ik searchstring er noget, tilføjes boardet hvis
                        // den har en værdi for den valgte property
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
