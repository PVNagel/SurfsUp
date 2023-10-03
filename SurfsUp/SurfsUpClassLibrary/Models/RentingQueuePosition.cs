using System.ComponentModel.DataAnnotations.Schema;

namespace SurfsUpClassLibrary.Models
{
    [NotMapped]
    public class RentingQueuePosition
    {
        public string SurfsUpUserId { get; set; }
        public int BoardId { get; set; }
        public DateTime QueueJoined { get; set; }
    }
}
