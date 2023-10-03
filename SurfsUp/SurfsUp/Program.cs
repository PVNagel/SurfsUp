using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfsUp.Data;
using SurfsUpClassLibrary.Models;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using SurfsUp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SurfsUpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfsUpContext") ?? throw new InvalidOperationException("Connection string 'SurfsUpContext' not found.")));

builder.Services.AddDefaultIdentity<SurfsUpUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SurfsUpContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ImageService>();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Localization Middleware
var culture = new CultureInfo("da-DK");
culture.NumberFormat.NumberDecimalSeparator = ",";
culture.NumberFormat.CurrencyDecimalSeparator = ",";
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new List<CultureInfo>
    {
        culture
    },
    SupportedUICultures = new List<CultureInfo>
    {
        culture
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SurfsUpContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SurfsUpUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    SeedData.Initialize(context);
    SeedData.SeedRoles(roleManager);
    SeedData.SeedUsers(userManager);
}

app.Run();
