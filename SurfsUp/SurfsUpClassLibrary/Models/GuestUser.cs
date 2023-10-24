using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SurfsUpClassLibrary.Models
{
    public class GuestUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Ip { get; set; }
        public ICollection<Renting>? Rentings { get; set; }
        public int RentingsCount { get; set; }
        public int RentingsMaxCount { get; } = 3;
    }
}
