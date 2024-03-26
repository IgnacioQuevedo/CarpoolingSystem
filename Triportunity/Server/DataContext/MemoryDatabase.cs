using System.Collections.Generic;
using Server.Objects.Domain;
using Server.Objects.Domain.ClientModels;

namespace Server.DataContext
{
    public class MemoryDatabase
    {
        public ICollection<Client> Clients { get; set; }
        public ICollection<Ride> Rides { get; set; }

        private static MemoryDatabase _database;
        private static readonly object padlock = new object();

        private MemoryDatabase()
        {
            Clients = new List<Client>();
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