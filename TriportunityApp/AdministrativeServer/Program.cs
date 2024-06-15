using Grpc.Net.Client;
using GrpcService;
using System;
using System.Linq;
using System.Collections.Generic;
using Grpc.Core;
using AdministrativeServer.EnumsModels;
using Google.Protobuf.WellKnownTypes;

namespace AdministrativeServer
{
    public class Program
    {
        private static AdministrativeService.AdministrativeServiceClient _client;

        static void Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7142");
            _client = new AdministrativeService.AdministrativeServiceClient(channel);

            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear();
                Console.WriteLine("Administrative Server Menu");
                Console.WriteLine("1. Create Ride");
                Console.WriteLine("2. Edit Ride");
                Console.WriteLine("3. Delete Ride");
                Console.WriteLine("4. View Ride Ratings");
                Console.WriteLine("5. View Next N Rides");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        CreateRide();
                        break;
                    case "2":
                        EditRide();
                        break;
                    case "3":
                        DeleteRide();
                        break;
                    case "4":
                        ViewRideRatings();
                        break;
                    case "5":
                        ViewNextNRides().Wait();
                        break;
                    case "6":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (keepRunning)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CreateRide()
        {
            Console.Clear();
            Console.WriteLine("Create Ride");
            // Collect ride details from the user
            Console.Write("Driver ID: ");
            string driverId = Console.ReadLine();
            int initialLocation = SelectCity("Initial Location");
            int endingLocation = SelectCity("Ending Location");

            string departureTime = InputDepartureTime();

            Console.Write("Available Seats: ");
            int availableSeats = int.Parse(Console.ReadLine());
            Console.Write("Price Per Person: ");
            double pricePerPerson = double.Parse(Console.ReadLine());
            Console.Write("Pets Allowed (Y/N): ");
            bool petsAllowed = Console.ReadLine().ToUpper() == "Y";
            Console.Write("Vehicle ID: ");
            string vehicleId = Console.ReadLine();

            var request = new RideRequest
            {
                RideId = Guid.NewGuid().ToString(),
                DriverId = driverId,
                InitialLocation = initialLocation,
                EndingLocation = endingLocation,
                DepartureTime = departureTime,
                AvailableSeats = availableSeats,
                PricePerPerson = pricePerPerson,
                PetsAllowed = petsAllowed,
                Published = true,
                VehicleId = vehicleId
            };

            try
            {
                var response = _client.AddRide(request);
                Console.WriteLine($"Ride creation status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error creating ride: {ex.Status.Detail}");
            }
        }

        static void EditRide()
        {
            Console.Clear();
            Console.WriteLine("Edit Ride");
            // Collect ride details from the user
            string rideId = SelectRide("Edit");
            if (rideId == null) return;

            Console.Write("Driver ID: ");
            string driverId = Console.ReadLine();
            int initialLocation = SelectCity("Initial Location");
            int endingLocation = SelectCity("Ending Location");

            string departureTime = InputDepartureTime();

            Console.Write("Available Seats: ");
            int availableSeats = int.Parse(Console.ReadLine());
            Console.Write("Price Per Person: ");
            double pricePerPerson = double.Parse(Console.ReadLine());
            Console.Write("Pets Allowed (Y/N): ");
            bool petsAllowed = Console.ReadLine().ToUpper() == "Y";
            Console.Write("Vehicle ID: ");
            string vehicleId = Console.ReadLine();

            var request = new RideRequest
            {
                RideId = rideId,
                DriverId = driverId,
                InitialLocation = initialLocation,
                EndingLocation = endingLocation,
                DepartureTime = departureTime,
                AvailableSeats = availableSeats,
                PricePerPerson = pricePerPerson,
                PetsAllowed = petsAllowed,
                VehicleId = vehicleId
            };

            try
            {
                var response = _client.UpdateRide(request);
                Console.WriteLine($"Ride update status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error updating ride: {ex.Status.Detail}");
            }
        }

        static void DeleteRide()
        {
            Console.Clear();
            Console.WriteLine("Delete Ride");
            // Collect ride ID from the user
            string rideId = SelectRide("Delete");
            if (rideId == null) return;

            var request = new RideRequest { RideId = rideId };

            try
            {
                var response = _client.DeleteRide(request);
                Console.WriteLine($"Ride deletion status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error deleting ride: {ex.Status.Detail}");
            }
        }

        static void ViewRideRatings()
        {
            Console.Clear();
            Console.WriteLine("View Ride Ratings");
            // Collect ride ID from the user
            string rideId = SelectRide("View Ratings");
            if (rideId == null) return;

            var request = new RideRequest { RideId = rideId };

            try
            {
                var response = _client.GetRideRatings(request);
                foreach (var rating in response.Ratings)
                {
                    Console.WriteLine($"Rating: {rating.Punctuation}, Comment: {rating.Comment}");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving ride ratings: {ex.Status.Detail}");
            }
        }

        static async Task ViewNextNRides()
        {
            Console.Clear();
            Console.WriteLine("View Next N Rides");
            // Collect number of rides from the user
            Console.Write("Enter the number of rides to view: ");
            int n = int.Parse(Console.ReadLine());

            var request = new StreamRidesRequest { Count = n };

            try
            {
                using var call = _client.StreamRides(request);
                int rideCount = 0;
                await foreach (var ride in call.ResponseStream.ReadAllAsync())
                {
                    rideCount++;
                    DisplayRide(ride);

                    if (rideCount == n)
                    {
                        Console.WriteLine("Press any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                    }
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving next rides: {ex.Status.Detail}");
            }
        }

        static void DisplayRide(Ride ride, int? index = null)
        {
            if (index.HasValue)
            {
                Console.WriteLine($"{index.Value}. ");
            }
            Console.WriteLine("==================================================");
            Console.WriteLine($"Ride ID: {ride.RideId}");
            Console.WriteLine($"Driver ID: {ride.DriverId}");
            Console.WriteLine($"Published: {ride.Published}");
            Console.WriteLine($"Initial Location: {ride.InitialLocation}");
            Console.WriteLine($"Ending Location: {ride.EndingLocation}");
            Console.WriteLine($"Departure Time: {DateTime.Parse(ride.DepartureTime):yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Available Seats: {ride.AvailableSeats}");
            Console.WriteLine($"Price Per Person: {ride.PricePerPerson}");
            Console.WriteLine($"Pets Allowed: {ride.PetsAllowed}");
            Console.WriteLine($"Vehicle ID: {ride.VehicleId}");
            Console.WriteLine("Passengers: " + string.Join(", ", ride.Passengers));
            Console.WriteLine("==================================================");
        }

        static string SelectRide(string action)
        {
            Console.WriteLine($"Select Ride to {action}");
            var request = new Empty();
            RidesResponse response;

            try
            {
                response = _client.GetAllRides(request);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving rides: {ex.Status.Detail}");
                return null;
            }

            int startIndex = 0;
            int totalRides = response.Rides.Count;

            while (true)
            {
                var ridesToShow = response.Rides.Skip(startIndex).Take(10).ToList();
                if (!ridesToShow.Any())
                {
                    Console.WriteLine("No more rides to show.");
                    break;
                }

                for (int i = 0; i < ridesToShow.Count; i++)
                {
                    DisplayRide(ridesToShow[i], startIndex + i + 1);
                }

                if (ridesToShow.Count < 10)
                {
                    break;
                }

                Console.Write("Show more rides? (Y/N): ");
                if (Console.ReadLine().ToUpper() != "Y")
                {
                    break;
                }

                startIndex += 10;
            }

            Console.Write("Enter the number of the ride: ");
            int rideIndex = int.Parse(Console.ReadLine()) - 1;

            if (rideIndex < 0 || rideIndex >= totalRides)
            {
                Console.WriteLine("Invalid ride number selected.");
                return null;
            }

            return response.Rides[rideIndex].RideId;
        }

        static int SelectCity(string cityType)
        {
            Console.WriteLine($"Select {cityType}");
            var cities = System.Enum.GetValues(typeof(CitiesEnum)).Cast<CitiesEnum>().ToList();

            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cities[i]}");
            }

            Console.Write("Enter the number of the city: ");
            int cityIndex = int.Parse(Console.ReadLine()) - 1;

            if (cityIndex < 0 || cityIndex >= cities.Count)
            {
                throw new ArgumentOutOfRangeException("Invalid city number selected.");
            }

            return (int)cities[cityIndex];
        }

        static string InputDepartureTime()
        {
            Console.Write("Enter Year of Departure: ");
            int year = int.Parse(Console.ReadLine());
            Console.Write("Enter Month of Departure: ");
            int month = int.Parse(Console.ReadLine());
            Console.Write("Enter Day of Departure: ");
            int day = int.Parse(Console.ReadLine());
            Console.Write("Enter Hour of Departure (24-hour format): ");
            int hour = int.Parse(Console.ReadLine());

            DateTime departureTime = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);
            return departureTime.ToString("o");
        }
    }
}
