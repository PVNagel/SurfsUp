using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using SurfsUpAPI.Data;
using SurfsUpAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SurfsUpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfsUpContext") ?? throw new InvalidOperationException("Connection string 'SurfsUpContext' not found.")));

builder.Services.AddScoped<ImageService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    option.ReportApiVersions = true;
    option.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-version"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7237")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddVersionedApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Version 1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API Version 2.0");
    });
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
