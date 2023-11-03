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
using SurfsUpAPI.Data;
using SurfsUpClassLibrary.Models;
using SurfsUpAPI.Services;
using Newtonsoft.Json;

namespace SurfsUpAPI.Controllers.v1
{
    [ApiController]
    [Route("/v{version:apiVersion}/[Controller]/[Action]")]
    [ApiVersion ("1.0")]
    public class BoardsAPIController : Controller
    {
        // her laver vi fields som vi sætter til vores services. 
        private readonly SurfsUpContext _context; //dbcontext er en scoped service

        public BoardsAPIController(
            //her bruger vi dependency injection til at hente vores services ind i controlleren igennem constructoren.
            SurfsUpContext context)
        {
            // sætter vores fields til vores injectede services
            _context = context;
        }

        // GET: by board by id
        [HttpGet]
        [Route("{id}")]
        public async Task<Board> GetById(int id)
        {
            var board = await _context.Boards.FindAsync(id);
            return board;
        }

        // GET: ALL Boards
        [HttpGet]
        public async Task<string> GetAllBoards()
        {
            var boardsList = await _context.Boards.Include(x => x.Rentings).Where(x => x.IsPremium == false).ToListAsync();
            return JsonConvert.SerializeObject(boardsList, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        
    }
}
