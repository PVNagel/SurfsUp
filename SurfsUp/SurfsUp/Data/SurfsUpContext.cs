﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Models;

namespace SurfsUp.Data
{
    public class SurfsUpContext : IdentityDbContext<SurfsUpUser>
    {
        public SurfsUpContext(DbContextOptions<SurfsUpContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<SurfsUp.Models.Board> Boards { get; set; } = default!;
        public DbSet<Image> Images { get; set; }

        public DbSet<SurfsUp.Models.Renting>? Renting { get; set; }
    }
}
