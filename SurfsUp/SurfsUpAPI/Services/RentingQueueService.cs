using SurfsUpClassLibrary.Models;
using System.Collections.Concurrent;
using System.Net;

namespace SurfsUpAPI.Services
{
    public static class RentingQueueService
    {
        private static ConcurrentBag<RentingQueuePosition> rentingQueuePositions = new ConcurrentBag<RentingQueuePosition>();

        public static bool AddPosition(RentingQueuePosition position)
        {
            try
            {
                rentingQueuePositions.Add(position);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static RentingQueuePosition GetPosition(string? userId, string? guestUserIp)
        {
            if(userId != null)
            {
                return rentingQueuePositions.FirstOrDefault(x => x.SurfsUpUserId == userId);
            }
            else
            {
                return rentingQueuePositions.FirstOrDefault(x => x.GuestUserIp == guestUserIp);
            }
        }

        public static bool RemovePosition(string? userId, string? guestUserIp)
        {
            if (userId != null)
            {
                var position = rentingQueuePositions.FirstOrDefault(x => x.SurfsUpUserId == userId);
                return rentingQueuePositions.TryTake(out position);
            }
            else
            {
                var position = rentingQueuePositions.FirstOrDefault(x => x.GuestUserIp == guestUserIp);
                return rentingQueuePositions.TryTake(out position);
            }

        }

        public static bool IsFirstPosition(RentingQueuePosition position)
        {
            foreach (var p in rentingQueuePositions)
            {
                if(p.BoardId == position.BoardId)
                {
                    if (position.QueueJoined > p.QueueJoined)
                    {
                        return false;
                    }
                }

            }
            return true;
        }
    }
}