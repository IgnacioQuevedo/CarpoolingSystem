using Grpc.Net.Client;
using GrpcService;
using System;
using System.Linq;
using System.Collections.Generic;
using Grpc.Core;
using AdministrativeServer.EnumsModels;

namespace AdministrativeServer
{
    public class Program
    {
        private static AdministrativeService.AdministrativeServiceClient _client;

        static void Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
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
                        ViewNextNRides();
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
            Console.Write("Departure Time (ISO Format): ");
            string departureTime = Console.ReadLine();
            Console.Write("Available Seats: ");
            int availableSeats = int.Parse(Console.ReadLine());
            Console.Write("Price Per Person: ");
            double pricePerPerson = double.Parse(Console.ReadLine());
            Console.Write("Pets Allowed (true/false): ");
            bool petsAllowed = bool.Parse(Console.ReadLine());
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
            Console.Write("Driver ID: ");
            string driverId = Console.ReadLine();
            int initialLocation = SelectCity("Initial Location");
            int endingLocation = SelectCity("Ending Location");
            Console.Write("Departure Time (ISO Format): ");
            string departureTime = Console.ReadLine();
            Console.Write("Available Seats: ");
            int availableSeats = int.Parse(Console.ReadLine());
            Console.Write("Price Per Person: ");
            double pricePerPerson = double.Parse(Console.ReadLine());
            Console.Write("Pets Allowed (true/false): ");
            bool petsAllowed = bool.Parse(Console.ReadLine());
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
                await foreach (var ride in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"Ride ID: {ride.RideId}, Departure Time: {ride.DepartureTime}");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving next rides: {ex.Status.Detail}");
            }
        }

        static string SelectRide(string action)
        {
            Console.WriteLine($"Select Ride to {action}");
            var request = new RidesRequest { Count = int.MaxValue };
            var response = _client.GetNextRides(request);

            for (int i = 0; i < response.Rides.Count; i++)
            {
                var ride = response.Rides[i];
                Console.WriteLine($"{i + 1}. Ride ID: {ride.RideId}, Initial Location: {ride.InitialLocation}, Ending Location: {ride.EndingLocation}, Departure Time: {ride.DepartureTime}");
            }

            Console.Write("Enter the number of the ride: ");
            int rideIndex = int.Parse(Console.ReadLine()) - 1;

            return response.Rides[rideIndex].RideId;
        }

        static int SelectCity(string cityType)
        {
            Console.WriteLine($"Select {cityType}");
            var cities = Enum.GetValues(typeof(CitiesEnum)).Cast<CitiesEnum>().ToList();

            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cities[i]}");
            }

            Console.Write("Enter the number of the city: ");
            int cityIndex = int.Parse(Console.ReadLine()) - 1;

            return (int)cities[cityIndex];
        }
    }
}
