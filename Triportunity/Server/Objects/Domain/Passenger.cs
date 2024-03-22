namespace Server.Objects.Domain
{
    public class Passenger
    {
        public Ride RideJoined { get; set; }
        public int SeatNumber { get; set; }


        public Passenger(Ride rideJoined, int seatNumber)
        {
            RideJoined = rideJoined;
            SeatNumber = seatNumber;
        }
    }
}