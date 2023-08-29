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
            using (var context = new SurfsUpContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfsUpContext>>()))
            {
                // Look for any movies.
                if (context.Board.Any())
                {
                    return;   // DB has been seeded
                }

                context.Board.AddRange(
                    new Board
                    {
                        Name = "The Minilog",
                        Length = 6,
                        Width = 21,
                        Thickness = 2.75,
                        Volume = 38.8,
                        Type = Models.Type.Shortboard,
                        Price = 565,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "The Wide Glider",
                        Length = 7.1,
                        Width = 21.75,
                        Thickness = 2.75,
                        Volume = 44.16,
                        Type = Models.Type.Funboard,
                        Price = 685,
                        Equipment = null,
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "The Golden Ratio",
                        Length = 6.3,
                        Width = 21.85,
                        Thickness = 2.9,
                        Volume = 43.22,
                        Type = Models.Type.Funboard,
                        Price = 695,
                        Equipment = null,
                        ImagePath = "d.jpg"
                    },


                    new Board
                    {
                        Name = "Mahi Mahi",
                        Length = 5.4,
                        Width = 20.75,
                        Thickness = 2.3,
                        Volume = 29.39,
                        Type = Models.Type.Fish,
                        Price = 645,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "The Emerald Glider",
                        Length = 9.2,
                        Width = 22.8,
                        Thickness = 2.8,
                        Volume = 65.4,
                        Type = Models.Type.Longboard,
                        Price = 895,
                        Equipment = null,
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "The Bomb",
                        Length = 5.5,
                        Width = 21,
                        Thickness = 2.5,
                        Volume = 33.7,
                        Type = Models.Type.Shortboard,
                        Price = 645,
                        Equipment = null,
                        ImagePath = "d.jpg"
                    },

                    new Board
                    {
                        Name = "Walden Magic",
                        Length = 9.6,
                        Width = 19.4,
                        Thickness = 3,
                        Volume = 80,
                        Type = Models.Type.Longboard,
                        Price = 1025,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "Naish One",
                        Length = 12.6,
                        Width = 30,
                        Thickness = 6,
                        Volume = 301,
                        Type = Models.Type.SUP,
                        Price = 854,
                        Equipment = "Paddle",
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "Six Tourer",
                        Length = 11.6,
                        Width = 32,
                        Thickness = 6,
                        Volume = 270,
                        Type = Models.Type.SUP,
                        Price = 611,
                        Equipment = "Paddle, Fin, Pump, Leash",
                        ImagePath = "d.jpg"
                    },

                    new Board
                    {
                        Name = "Naish Maliko",
                        Length = 14,
                        Width = 25,
                        Thickness = 6,
                        Volume = 330,
                        Type = Models.Type.SUP,
                        Price = 1304,
                        Equipment = "Paddle, Fin, Pump, Leash",
                        ImagePath = "a.jpg"
                    }

                );
                context.SaveChanges();
            }
        }
    }
}
