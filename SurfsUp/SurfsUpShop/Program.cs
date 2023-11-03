using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SurfsUpClassLibrary.Product;
using SurfsUpClassLibrary.ShoppingCart;
using SurfsUpClassLibrary.Storage;
//using SurfsUpShop.Libraries.Services.Product;
//using SurfsUpShop.Libraries.Services.Storage;
//using SurfsUpShop.Libraries.Services.ShoppingCart;
using SurfsUpShop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IProductService, ProductService>();

await builder.Build().RunAsync();
