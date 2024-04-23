using System.Linq;
using System.Threading;
using Server.DataContext;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.UserModels;

namespace Server.Repositories
{
    public class ClientRepository
    {
        private readonly object _monitor = new object();
        private int _readersCount = 0;

        public void RegisterClient(User clientToRegister)
        {
            lock (_monitor)
            {
                if (!UsernameRegistered(clientToRegister.Username))
                {
                    MemoryDatabase.GetInstance().Users.Add(clientToRegister);
                }
            }
        }

        private bool UsernameRegistered(string username)
        {
            var clientWithThatUsername = FindClientViaUsername(username);

            if (clientWithThatUsername != null)
            {
                return false;
            }

            return true;
        }

        private bool Login(string username, string password)
        {
            var possibleLogin = FindClientViaUsername(username);

            if (possibleLogin.Password.Equals(password))
            {
                return true;
            }

            return false;
        }

        public User FindClientViaUsername(string usernameOfClient)
        {
            var clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Username.Equals(usernameOfClient));

            return clientFound;
        }

        public void UpdateClient(User clientWithUpdates)
        {
            lock (clientWithUpdates)
            {
                foreach (var client in MemoryDatabase.GetInstance().Users)
                {
                    if (client.Username.Equals(clientWithUpdates.Username))
                    {
                        client.Password = clientWithUpdates.Password;
                        client.DriverAspects = clientWithUpdates.DriverAspects;
                    }
                }
            }
        }
    }
}