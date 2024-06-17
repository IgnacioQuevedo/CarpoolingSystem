using System.Collections.Generic;
using System.Threading.Tasks;
using MainServer.Objects.Domain;
using MainServer.Objects.Domain.UserModels;
using MainServer.Objects.Domain.VehicleModels;

namespace MainServer.DataContext
{
    public class MemoryDatabase
    {
        public ICollection<User> Users { get; set; }
        public ICollection<Ride> Rides { get; set; }

        private static MemoryDatabase _database;
        private static readonly object padlock = new object();

        private MemoryDatabase()
        {
            Users = new List<User>();
            Rides = new List<Ride>();
        }
        public static MemoryDatabase GetInstance()
        {
            lock (padlock)
            {
                if (_database is null)
                {
                    _database = new MemoryDatabase();
                }

                return _database;
            }
          
        }
    }
}