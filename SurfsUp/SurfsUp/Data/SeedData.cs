﻿using Microsoft.AspNetCore.Hosting;
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
            //List<Board> newBoards = new List<Board>();

            using (var context = new SurfsUpContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfsUpContext>>()))
            {
                // Look for any movies.
                if (!context.Boards.Any())
                {
                    context.Boards.AddRange(new List<Board>
                {
                     new Board
                    {
                        Name = "The Minilog",
                        Length = 6,
                        Width = 21,
                        Thickness = 2.75,
                        Volume = 38.8,
                        Type = Models.TypeEnum.Shortboard,
                        Price = 565,
                        Equipment = null
                    },
                     new Board
    {
        Name = "The Wide Glider",
        Length = 7.1,
        Width = 21.75,
        Thickness = 2.75,
        Volume = 44.16,
        Type = Models.TypeEnum.Funboard,
        Price = 685,
        Equipment = null
    },
                     new Board
    {
        Name = "The Golden Ratio",
        Length = 6.3,
        Width = 21.85,
        Thickness = 2.9,
        Volume = 43.22,
        Type = Models.TypeEnum.Funboard,
        Price = 695,
        Equipment = null
    },
                     new Board
    {
        Name = "Mahi Mahi",
        Length = 5.4,
        Width = 20.75,
        Thickness = 2.3,
        Volume = 29.39,
        Type = Models.TypeEnum.Fish,
        Price = 645,
        Equipment = null
    },
                     new Board
    {
        Name = "The Emerald Glider",
        Length = 9.2,
        Width = 22.8,
        Thickness = 2.8,
        Volume = 65.4,
        Type = Models.TypeEnum.Longboard,
        Price = 895,
        Equipment = null
    },
                     new Board
    {
        Name = "The Bomb",
        Length = 5.5,
        Width = 21,
        Thickness = 2.5,
        Volume = 33.7,
        Type = Models.TypeEnum.Shortboard,
        Price = 645,
        Equipment = null
    },
                     new Board
    {
        Name = "Walden Magic",
        Length = 9.6,
        Width = 19.4,
        Thickness = 3,
        Volume = 80,
        Type = Models.TypeEnum.Longboard,
        Price = 1025,
        Equipment = null
    },
                     new Board
    {
        Name = "Naish One",
        Length = 12.6,
        Width = 30,
        Thickness = 6,
        Volume = 301,
        Type = Models.TypeEnum.SUP,
        Price = 854,
        Equipment = "Paddle"
    },
                     new Board
    {
        Name = "Six Tourer",
        Length = 11.6,
        Width = 32,
        Thickness = 6,
        Volume = 270,
        Type = Models.TypeEnum.SUP,
        Price = 611,
        Equipment = "Fin, Paddle, Pump, Leash"
    },
                     new Board
    {
        Name = "Naish Maliko",
        Length = 14,
        Width = 25,
        Thickness = 6,
        Volume = 330,
        Type = Models.TypeEnum.SUP,
        Price = 1304,
        Equipment = "Fin, Paddle, Pump, Leash"
    }
            });
                }

                if (!context.Images.Any())
                {
                    context.Images.AddRange(new List<Image>
                {
                    new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 1
                    },
                                        new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 2
                    },
                                                            new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 3
                    },
                                                                                new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 4
                    },
                                                                                                    new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 5
                    },
                                                                                                                        new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 6
                    },
                                                                                                                                            new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 7
                    },
                                                                                                                                                                new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 8
                    },
                                                                                                                                                                                    new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 9
                    },
                                                                                                                                                                                                        new Image
                    {
                        Path = "/SeedImages/a.jpg",
                        BoardId = 10
                    },
                });
                }

                


                context.SaveChanges();

                //var _webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                //foreach (var board in newBoards)
                //{
                //    string rootPath = _webHostEnvironment.WebRootPath;

                //    var filePath = Path.Combine(rootPath + "/Images/" + board.Id);

                //    if (!Directory.Exists(filePath))
                //    {
                //        Directory.CreateDirectory(filePath);
                //    }

                //    filePath = Path.Combine(rootPath + "/Images/" + board.Id, "a.jpg");

                //    var sourceFilePath = Path.Combine(rootPath + "/SeedImages", "a.jpg");

                //    System.IO.File.Copy(sourceFilePath, filePath);
                //}
            }
        }
    }
}

