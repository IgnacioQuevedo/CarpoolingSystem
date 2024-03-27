using System.Linq;
using Server.DataContext;
using Server.Objects.Domain.ClientModels;

namespace Server.Repositories
{
    public class ClientRepository
    {
        private readonly object _registerLock = new object();

        public void RegisterClient(Client clientToRegister)
        {
            lock (_registerLock)
            {
                if (!UsernameRegistered(clientToRegister.Username))
                {
                    MemoryDatabase.GetInstance().Clients.Add(clientToRegister);
                }
            }
        }

        private bool UsernameRegistered(string username)
        {
            var clientWithThatUsername = MemoryDatabase.GetInstance().Clients.Where(x => x.Username.Equals(username));

            if (clientWithThatUsername.Any())
            {
                return false;
            }

            return true;
        }
        private bool ClientIsLogged(string username,string password)
        {
            var possibleLogin = MemoryDatabase.GetInstance().Clients.
                Where(x => x.Username == username);

            if (possibleLogin.First().Password.Equals(password))
            {
                return true;
            }
            return false;
        }
    }
}