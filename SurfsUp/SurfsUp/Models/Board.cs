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
        [DataType(DataType.Text)]
        public string? Name { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+,[0-9]+$", ErrorMessage = "Du må kun skrive tal og komma som decimal seperator.")]
        public double Length { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+,[0-9]+$", ErrorMessage = "Du må kun skrive tal og komma som decimal seperator.")]
        public double Width { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+,[0-9]+$", ErrorMessage = "Du må kun skrive tal og komma som decimal seperator.")]
        public double Thickness { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+,[0-9]+$", ErrorMessage = "Du må kun skrive tal og komma som decimal seperator.")]
        public double Volume { get; set; }
        [Required]
        public Type Type { get; set; }
        [Required]
        [DataType (DataType.Currency)]
        [RegularExpression(@"^[0-9]+,[0-9]+$", ErrorMessage = "Du må kun skrive tal og komma som decimal seperator.")]
        public double Price { get; set; }
        [DataType(DataType.Text)]
        public string? Equipment { get; set; }
        [NotMapped]
        public IList<IFormFile>? Attachments { get; set; }
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
