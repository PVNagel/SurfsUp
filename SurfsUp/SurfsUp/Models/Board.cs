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
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public string? Length { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public string? Width { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public string? Thickness { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public string? Volume { get; set; }
        [Required]
        public TypeEnum Type { get; set; }
        [Required]
        [DataType (DataType.Currency)]
        public decimal Price { get; set; }
        [DataType(DataType.Text)]
        public string? Equipment { get; set; }
        [NotMapped]
        public IList<IFormFile>? Attachments { get; set; }
    }
    public enum TypeEnum
    {
        Shortboard,
        Funboard,
        Fish,
        Longboard,
        SUP
    }
}
