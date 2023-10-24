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

namespace SurfsUpAPI.Controllers.v2
{
    [ApiController]
    [Route("/v{version:apiVersion}/[Controller]/[Action]")]
    [ApiVersion("2.0")]
    public class RentingsAPIController : Controller
    {
        private readonly SurfsUpContext _context;
        //private readonly UserManager<SurfsUpUser> _userManager;

        public RentingsAPIController(SurfsUpContext context) //UserManager<SurfsUpUser> _userManager)
        {
            //_userManager = _userManager;
            _context = context;
        }


        
    }
}
