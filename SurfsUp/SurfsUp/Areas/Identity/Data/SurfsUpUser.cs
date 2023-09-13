using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SurfsUp.Models;

namespace SurfsUp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the SurfsUpUser class
public class SurfsUpUser : IdentityUser
{
    public ICollection<Renting>? Rentings { get; set; }
}

