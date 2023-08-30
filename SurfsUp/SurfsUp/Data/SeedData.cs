using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfsUp.Models;
using System;
using System.Linq;

namespace SurfsUp.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            List<Board> newBoards = new List<Board>();

            using (var context = new SurfsUpContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfsUpContext>>()))
            {
                // Look for any movies.
                if (context.Board.Any())
                {
                    return;   // DB has been seeded
                }


                newBoards.Add(context.Board.Add(
                    new Board
                    {
                        Name = "The Minilog",
                        Length = "6",
                        Width = "21",
                        Thickness = "2,75",
                        Volume = "38.8",
                        Type = Models.Type.Shortboard,
                        Price = 565,
                        Equipment = null
                    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);


                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                newBoards.Add(context.Board.Add(
    new Board
    {
        Name = "The Minilog",
        Length = "6",
        Width = "21",
        Thickness = "2,75",
        Volume = "38.8",
        Type = Models.Type.Shortboard,
        Price = 565,
        Equipment = null
    }).Entity);

                context.SaveChanges();

                var _webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                foreach (var board in newBoards)
                {
                    string rootPath = _webHostEnvironment.WebRootPath;

                    var filePath = Path.Combine(rootPath + "/Images/" + board.Id);

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    filePath = Path.Combine(rootPath + "/Images/" + board.Id, "a.jpg");

                    var sourceFilePath = Path.Combine(rootPath + "/SeedImages", "a.jpg");

                    System.IO.File.Copy(sourceFilePath, filePath);
                }
            }
        }
    }
}

