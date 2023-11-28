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
using SurfsUpAPI.Data;
using SurfsUpClassLibrary.Models;
using SurfsUpAPI.Services;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace SurfsUpAPI.Controllers.v1
{
    [ApiController]
    [Route("/v{version:apiVersion}/[Controller]/[Action]")]
    [ApiVersion("1.0")]
    public class RentingsAPIController : Controller
    {
        private readonly SurfsUpContext _context;
        //private readonly UserManager<SurfsUpUser> _userManager;

        public RentingsAPIController(SurfsUpContext context) //UserManager<SurfsUpUser> _userManager)
        {
            //_userManager = _userManager;
            _context = context;
        }


        // GET: Rentings
        [HttpGet]
        public async Task<string> Get(string? userId, string? guestUserIp)
        {
            List<Renting> rentings;
            if(userId != null)
            {
                rentings = User.IsInRole("Admin") ?
                await _context.Renting.Include(r => r.Board).Include(r => r.SurfsUpUser).ToListAsync() :
                await _context.Renting.Include(r => r.Board).Include(r => r.SurfsUpUser).Where(x => x.SurfsUpUserId == userId).ToListAsync();
            }
            else
            {
                rentings = await _context.Renting.Include(r => r.Board).Where(x => x.GuestUserIp == guestUserIp).ToListAsync();
            }
            return JsonConvert.SerializeObject(rentings, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

     

        [HttpPost]
        public async Task<IActionResult> AddQueuePosition(QueuePositionDataTransferObject queueObject)
        {
            if(queueObject.GuestUserIp != null)
            {
                bool added = RentingQueueService.AddPosition(new RentingQueuePosition()
                {
                    GuestUserIp = queueObject.GuestUserIp,
                    QueueJoined = DateTime.Now,
                    BoardId = queueObject.BoardId
                });
                if (!added)
                {
                    return BadRequest();
                }
            }
            else
            {
                bool added = RentingQueueService.AddPosition(new RentingQueuePosition()
                {
                    SurfsUpUserId = queueObject.UserId,
                    QueueJoined = DateTime.Now,
                    BoardId = queueObject.BoardId
                });
                if (!added)
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Renting renting)
        {
            try
            {
                //*****ATTENTION NEED NEW FIX*****
                //if(renting.GuestUserIp == null && renting.SurfsUpUserId == null)
                //{
                //    return BadRequest();
                //}
                //else
                //{
                //    if(renting.GuestUserIp != null)
                //    {
                //        var guestUser = await _context.GuestUsers.FindAsync(renting.GuestUserIp);
                //        if(guestUser == null)
                //        {
                //            return BadRequest();
                //        }
                //    }
                //    else
                //    {
                //        if (User.Identity.IsAuthenticated)
                //        {
                //            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != renting.SurfsUpUserId)
                //            {
                //                return BadRequest();
                //            }
                //        }
                //        else
                //        {
                //            return BadRequest();
                //        }
                //    }
                //}

                if (ModelState.IsValid)
                {
                    renting.StartDate = DateTime.Now;
                    var rentings = _context.Renting.Where(x => x.BoardId == renting.BoardId);
                    foreach (var item in rentings)
                    {
                        // hvis nye rentings startdate er før item enddate
                        // ELLER renting startdate er efter item startdate og renting enddate er før item enddate 
                        if (renting.StartDate < item.EndDate || renting.StartDate > item.StartDate && renting.EndDate < item.EndDate)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Unable to create new renting because another renting has already been created.");

                            return BadRequest(SerializeModelState(ModelState));
                        }
                    }

                    if (renting.EndDate < renting.StartDate.AddMinutes(55))
                    {
                        ModelState.AddModelError(string.Empty,
                           "An end date is required that is atleast 1 hour long");

                        return BadRequest(SerializeModelState(ModelState));
                    }

                    if (!RentingQueueService.IsFirstPosition(RentingQueueService.GetPosition(renting.SurfsUpUserId, renting.GuestUserIp)))
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to create new renting because another user is currently infront of you in the queue");

                        return BadRequest(SerializeModelState(ModelState));
                    }

                    if (!RentingQueueService.RemovePosition(renting.SurfsUpUserId, renting.GuestUserIp))
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to remove you from the renting queue, please try again.");

                        return BadRequest(SerializeModelState(ModelState));
                    }


                    _context.Add(renting);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(Create), new { id = renting.Id }, renting);
                }
                return BadRequest(ModelState);
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        //Bliver kaldt fra userLeavesRentingPage.js i /renting/create view, hvis man går væk fra siden uden at trykke create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQueuePosition([FromBody] JObject requestBody)
        {
            string userId = null;
            string clientIP = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                clientIP = requestBody.Value<string>("clientIP");
            }

            if (RentingQueueService.RemovePosition(userId, clientIP))
            {
                return Ok(new { Message = "Anmodningen blev behandlet med succes." });
            }
            return BadRequest(new { Message = "Anmodningen kunne ikke fuldføres." });
        }

        private bool RentingExists(int id)
        {
            return (_context.Renting?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string SerializeModelState(ModelStateDictionary modelState)
        {
            var errors = modelState
                            .Where(x => x.Value.Errors.Any())
                            .Select(x => new ModelStateError
                            {
                                Key = x.Key,
                                ErrorMessage = x.Value.Errors.First().ErrorMessage
                            })
                            .ToList();

            // Serialize the errors to JSON
            return JsonConvert.SerializeObject(errors);
        }
    }
}
