using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace SurfsUpClassLibrary.Models
{
    [NotMapped]
    public class RentingQueuePosition
    {
        public string SurfsUpUserId { get; set; }
        public string GuestUserIp { get; set; }
        public int BoardId { get; set; }
        public DateTime QueueJoined { get; set; }
    }
}
