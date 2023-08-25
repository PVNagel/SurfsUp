﻿using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;

namespace SurfsUp.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        
        [Required] 
        public string Name { get; set; }
        [Required]
        public float Length { get; set; }
        [Required]
        public float Width { get; set; }
        [Required]
        public float Thickness { get; set; }
        [Required]
        public float Volume { get; set; }
        [Required]
        public Type Type { get; set; }
        [Required]
        [DataType (DataType.Currency)]
        public float Price { get; set; }
        public string? Equipment { get; set; }

        public string Image { get; set; } = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/s326152794241300969_p347_i16_w5000.jpeg?width=640";
    }
    public enum Type
    {
        Shortboard,
        Funboard,
        Fish,
        Longboard,
        SUP
    }
}
