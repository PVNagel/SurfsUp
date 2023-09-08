using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfsUp.Data;
using SurfsUp.Areas.Identity.Data;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SurfsUpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfsUpContext") ?? throw new InvalidOperationException("Connection string 'SurfsUpContext' not found.")));

builder.Services.AddDefaultIdentity<SurfsUpUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<SurfsUpContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var culture = new CultureInfo("da-DK");
culture.NumberFormat.NumberDecimalSeparator = ",";
culture.NumberFormat.CurrencyDecimalSeparator = ",";

//var culture2 = new CultureInfo("en-US");
//culture2.NumberFormat.NumberDecimalSeparator = ".";
//culture2.NumberFormat.CurrencyDecimalSeparator = ".";

// Configure the Localization middleware
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
app.Run();
