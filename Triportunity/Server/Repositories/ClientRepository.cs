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
                MemoryDatabase.GetInstance().Clients.Add(clientToRegister);
            }
        }
        
        
        
    }
}