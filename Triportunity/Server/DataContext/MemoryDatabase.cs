using System.Collections.Generic;
using Server.Objects.Domain;
using Serverg.Objects.Domain.ClientModels;

namespace Server.DataContext
{
    public class MemoryDatabase
    {
        public ICollection<Client> Clients { get; set; }
        public ICollection<Ride> Rides { get; set; }

        private static MemoryDatabase _database;

        private MemoryDatabase()
        {
            Clients = new List<Client>();
            Rides = new List<Ride>();
        }
        public static MemoryDatabase GetInstance()
        {
            if (_database is null)
            {
                _database = new MemoryDatabase();
            }

            return _database;
        }
        
        
    }
}