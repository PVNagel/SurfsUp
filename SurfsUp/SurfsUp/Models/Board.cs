using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public double? Length { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public double? Width { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public double? Thickness { get; set; }
        [Required]
        [RegularExpression(@"^[1-9][0-9]*(?:,[0-9]+)?$", ErrorMessage = "Du må kun bruge tal og 1 enkelt komma.")]
        public double? Volume { get; set; }
        [Required(ErrorMessage = "You need to select a type")]
        public TypeEnum Type { get; set; }
        [Required]
        [DataType (DataType.Currency)]
        public decimal Price { get; set; }
        [DataType(DataType.Text)]
        public string? Equipment { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // et board kan have mange rentings og en renting kan kun have 1 board. en-til-mange db relation
        public ICollection<Renting>? Rentings { get; set; }
        // et board kan have mange images og et image kan kun have 1 board. en-til-mange db relation
        public ICollection<Image>? Images { get; set; }

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
