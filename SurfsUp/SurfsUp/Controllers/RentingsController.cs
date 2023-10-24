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
    public class RentingsController : Controller
    {
        private readonly SurfsUpContext _context; //dbcontext er en scoped service
        public RentingsController(SurfsUpContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userId = null;
            string guestUserIp = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                guestUserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            HttpClient client = new HttpClient();

            string url = $"https://localhost:7022/v1/RentingsAPI/Get?userId={userId}&guestUserIp={guestUserIp}";

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
            QueuePositionDataTransferObject queuePositionDataTransfer;
            var board = await _context.Boards.FindAsync(boardId);
            string userId = null;
            string guestUserIp = null;
            
            if(User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                queuePositionDataTransfer = new QueuePositionDataTransferObject { BoardId = boardId, UserId = userId };
            }
            else
            {
                guestUserIp = HttpContext.Connection.RemoteIpAddress.ToString();
                var guestUser = await _context.GuestUsers.FindAsync(guestUserIp);
                if(guestUser != null)
                {
                    if(guestUser.RentingsCount < guestUser.RentingsMaxCount)
                    {
                        guestUser.RentingsCount += 1;
                        _context.Update(guestUser);
                        await _context.SaveChangesAsync();
                        queuePositionDataTransfer = new QueuePositionDataTransferObject { BoardId = boardId, GuestUserIp = guestUserIp };
                    }
                    else
                    {
                        return BadRequest("Max rentings reached as guest. please log in for unlimited rentings. FOR FREEE BRO LOL XD");
                    }
                }
                else
                {
                        var entity = _context.GuestUsers.Add(new GuestUser { Ip = guestUserIp, RentingsCount = 1 });
                        await _context.SaveChangesAsync();
                    queuePositionDataTransfer = new QueuePositionDataTransferObject { BoardId = boardId, GuestUserIp = guestUserIp };

                }
            }
            HttpClient client = new HttpClient();
            string url = $"https://localhost:7022/v1/RentingsAPI/AddQueuePosition";
            var response = await client.PostAsJsonAsync(url, queuePositionDataTransfer);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Could not add Queue Position - FUCK");
            }

            Renting renting;
            if(userId != null)
            {
                renting = new Renting { BoardId = boardId, SurfsUpUserId = userId, EndDate = DateTime.Now };
            }
            else
            {
                renting = new Renting { BoardId = boardId, GuestUserIp = guestUserIp, EndDate = DateTime.Now };
            }
            
            ViewData["BoardName"] = board.Name;
            return View(renting);
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardId,StartDate,EndDate,SurfsUpUserId,GuestUserIp")] Renting renting)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = $"https://localhost:7022/v1/RentingsAPI/Create";
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

        [HttpGet]
        public string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
