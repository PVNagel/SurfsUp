using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfsUp.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        
        [Required] 
        public string? Name { get; set; }
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
        [NotMapped]
        public IList<IFormFile> Attachments { get; set; }

        public string ImagePath { get; set; }
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
