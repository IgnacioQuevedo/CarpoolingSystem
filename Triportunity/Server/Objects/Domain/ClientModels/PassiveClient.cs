using Serverg.Objects.Domain.ClientModels;

namespace Server.Objects.Domain.ClientModels
{
    public class PassiveClient : Client
    {
        public PassiveClient(string username, string password) : base(username, password)
        {
        }
    }
}