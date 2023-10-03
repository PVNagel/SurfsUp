using SurfsUpClassLibrary.Models;
using System.Collections.Concurrent;

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