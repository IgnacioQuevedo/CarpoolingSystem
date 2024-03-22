using Serverg.Objects.Domain.ClientModels;

namespace Server.Objects.Domain.ClientModels
{
    public class Passenger : Client
    {
        public int SeatNumber { get; set; }
        
        public Passenger(string username,string password, int seatNumber) : base(username,password)
        {
            SeatNumber = seatNumber;
        }
    }
}