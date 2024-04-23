using Server.DataContext;
using System.Linq;
using Server.Objects.Domain.UserModels;

namespace Server.Repositories
{
    public class UserRepository
    {
        public void CreateUser(User userToAdd)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Users.Add(userToAdd);
            LockManager.StopWriting();
        }
        
        public void RegisterClient(User clientToRegister)
        {
            if (!UsernameRegistered(clientToRegister.Username))
            {
                MemoryDatabase.GetInstance().Users.Add(clientToRegister);
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
]