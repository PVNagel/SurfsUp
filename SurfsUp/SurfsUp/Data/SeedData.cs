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
                        Thickness = 2.75F,
                        Volume = 38.8F,
                        Type = Models.Type.Shortboard,
                        Price = 565,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "The Wide Glider",
                        Length = 7.1F,
                        Width = 21.75F,
                        Thickness = 2.75F,
                        Volume = 44.16F,
                        Type = Models.Type.Funboard,
                        Price = 685,
                        Equipment = null,
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "The Golden Ratio",
                        Length = 6.3F,
                        Width = 21.85F,
                        Thickness = 2.9F,
                        Volume = 43.22F,
                        Type = Models.Type.Funboard,
                        Price = 695,
                        Equipment = null,
                        ImagePath = "d.jpg"
                    },


                    new Board
                    {
                        Name = "Mahi Mahi",
                        Length = 5.4F,
                        Width = 20.75F,
                        Thickness = 2.3F,
                        Volume = 29.39F,
                        Type = Models.Type.Fish,
                        Price = 645,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "The Emerald Glider",
                        Length = 9.2F,
                        Width = 22.8F,
                        Thickness = 2.8F,
                        Volume = 65.4F,
                        Type = Models.Type.Longboard,
                        Price = 895,
                        Equipment = null,
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "The Bomb",
                        Length = 5.5F,
                        Width = 21F,
                        Thickness = 2.5F,
                        Volume = 33.7F,
                        Type = Models.Type.Shortboard,
                        Price = 645,
                        Equipment = null,
                        ImagePath = "d.jpg"
                    },

                    new Board
                    {
                        Name = "Walden Magic",
                        Length = 9.6F,
                        Width = 19.4F,
                        Thickness = 3F,
                        Volume = 80F,
                        Type = Models.Type.Longboard,
                        Price = 1025,
                        Equipment = null,
                        ImagePath = "a.jpg"
                    },

                    new Board
                    {
                        Name = "Naish One",
                        Length = 12.6F,
                        Width = 30F,
                        Thickness = 6F,
                        Volume = 301F,
                        Type = Models.Type.SUP,
                        Price = 854,
                        Equipment = "Paddle",
                        ImagePath = "c.jpg"
                    },

                    new Board
                    {
                        Name = "Six Tourer",
                        Length = 11.6F,
                        Width = 32F,
                        Thickness = 6F,
                        Volume = 270F,
                        Type = Models.Type.SUP,
                        Price = 611,
                        Equipment = "Paddle, Fin, Pump, Leash",
                        ImagePath = "d.jpg"
                    },

                    new Board
                    {
                        Name = "Naish Maliko",
                        Length = 14F,
                        Width = 25F,
                        Thickness = 6F,
                        Volume = 330F,
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
