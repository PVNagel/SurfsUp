using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SurfsUpClassLibrary.Models
{
    public class QueuePositionDataTransferObject
    {
        public int BoardId { get; set; }
        public string? UserId { get; set; }
        public string? GuestUserIp { get; set; }
    }
}
