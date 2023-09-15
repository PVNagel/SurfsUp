using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfsUp.Data;
using SurfsUp.Areas.Identity.Data;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using SurfsUp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SurfsUpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfsUpContext") ?? throw new InvalidOperationException("Connection string 'SurfsUpContext' not found.")));


builder.Services.AddDefaultIdentity<SurfsUpUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SurfsUpContext>();

builder.Services.AddScoped<ImageService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery();

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

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin" };

    foreach(var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SurfsUpUser>>();
    var userManager2 = scope.ServiceProvider.GetRequiredService<UserManager<SurfsUpUser>>();


    string email = "admin@admin.com";
    string password = "Password123.";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new SurfsUpUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }

    string email2 = "admin2@admin2.com";
    string password2 = "Password1234.";

    if (await userManager2.FindByEmailAsync(email2) == null)
    {
        var user2 = new SurfsUpUser();
        user2.UserName = email2;
        user2.Email = email2;
        user2.EmailConfirmed = true;

        await userManager.CreateAsync(user2, password2);

        await userManager.AddToRoleAsync(user2, "Admin");
    }

}



app.Run();
