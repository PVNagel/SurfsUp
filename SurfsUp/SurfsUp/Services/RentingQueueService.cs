using SurfsUp.Models;
using System.Collections.Concurrent;

namespace SurfsUp.Services
{
    public static class RentingQueueService
    {
        private static ConcurrentBag<RentingQueuePosition> rentingQueuePositions = new ConcurrentBag<RentingQueuePosition>();

        public static void AddPosition(RentingQueuePosition position)
        {
            rentingQueuePositions.Add(position);
        }

        public static RentingQueuePosition GetPosition(string userId)
        {
            return rentingQueuePositions.FirstOrDefault(x => x.SurfsUpUserId == userId);
        }

        public static bool RemovePosition(string surfsUpUserId)
        {
            var position = rentingQueuePositions.FirstOrDefault(x => x.SurfsUpUserId == surfsUpUserId);
            return rentingQueuePositions.TryTake(out position);
        }

        public static bool IsFirstPosition(RentingQueuePosition position)
        {
            foreach (var p in rentingQueuePositions)
            {
                if (position.QueueJoined > p.QueueJoined)
                {
                    return false;
                }
            }
            return true;
        }
    }
}