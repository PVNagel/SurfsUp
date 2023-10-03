using Humanizer.Localisation;
using Humanizer;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Common;
using SurfsUp.Data;
using SurfsUp.Services;
using SurfsUpClassLibrary.Models;
using System.Diagnostics;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;

namespace SurfsUp.Controllers
{
    [Authorize]
    public class RentingsController : Controller
    {
        private readonly SurfsUpContext _context; //dbcontext er en scoped service
        public RentingsController(SurfsUpContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            HttpClient client = new HttpClient();

            string url = $"https://localhost:7022/RentingsAPI/Get/{userId}";

            var rentings = await client.GetFromJsonAsync<List<Renting>>(url);
            if(rentings == null)
            {
                return BadRequest("rentings is null");
            }

            return View(rentings);

        }

        [Route("/Rentings/Create")]
        [HttpGet]
        public async Task<IActionResult> Create(int boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            HttpClient client = new HttpClient();
            QueuePositionDataTransferObject queuePositionDataTransfer = new QueuePositionDataTransferObject { BoardId = boardId, UserId = userId };
            string url = $"https://localhost:7022/RentingsAPI/AddQueuePosition";
            var response = await client.PostAsJsonAsync(url, queuePositionDataTransfer);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Could not add Queue Position - FUCK");
            }
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
                HttpClient client = new HttpClient();
                string url = $"https://localhost:7022/RentingsAPI/Create";
                var response = await client.PostAsJsonAsync(url, renting);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorJson = await response.Content.ReadAsStringAsync();
                        var errors = JsonConvert.DeserializeObject<List<ModelStateError>>(errorJson);
                        foreach(var error in errors)
                        {
                            ModelState.AddModelError(error.Key, error.ErrorMessage);
                        }
                        return View(renting);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
