using Serverg.Objects.Domain.ClientModels;

namespace Server.Objects.Domain.ClientModels
{
    public class Passenger : Client
    {
        public Ride RideJoined { get; set; }
        public int SeatNumber { get; set; }
        
        public Passenger(string username,string password,Ride rideJoined, int seatNumber) : base(username,password)
        {
            RideJoined = rideJoined;
            SeatNumber = seatNumber;
        }
    }
}