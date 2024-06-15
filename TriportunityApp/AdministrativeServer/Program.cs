using Grpc.Net.Client;
using GrpcService;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using AdministrativeServer.EnumsModels;
using Google.Protobuf.WellKnownTypes;

namespace AdministrativeServer
{
    public class Program
    {
        private static AdministrativeService.AdministrativeServiceClient _client;

        static async Task Main(string[] args)
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
                        await CreateRide();
                        break;
                    case "2":
                        await EditRide();
                        break;
                    case "3":
                        await DeleteRide();
                        break;
                    case "4":
                        await ViewRideRatings();
                        break;
                    case "5":
                        await ViewNextNRides();
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

        static async Task CreateRide()
        {
            Console.Clear();
            Console.WriteLine("Create Ride");
            try
            {
                // Collect ride details from the user
                Console.Write("Driver ID: ");
                string driverId = Console.ReadLine();
                int initialLocation = SelectCity("Initial Location");
                int endingLocation = SelectCity("Ending Location");

                string departureTime = InputDepartureTime();

                int availableSeats = PickAmountOfAvailableSeats();
                double pricePerPerson = IntroducePricePerPerson();
                bool petsAllowed = DecideIfPetsAreAllowed();
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

                var response = await _client.AddRideAsync(request);
                Console.WriteLine($"Ride creation status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error creating ride: {ex.Status.Detail}");
            }
        }

        static async Task EditRide()
        {
            Console.Clear();
            Console.WriteLine("Edit Ride");
            try
            {
                // Collect ride details from the user
                string rideId = SelectRide("Edit");
                if (rideId == null) return; // Return to main menu if no rides are available

                Console.Write("Driver ID: ");
                string driverId = Console.ReadLine();
                int initialLocation = SelectCity("Initial Location");
                int endingLocation = SelectCity("Ending Location");

                string departureTime = InputDepartureTime();

                int availableSeats = PickAmountOfAvailableSeats();
                double pricePerPerson = IntroducePricePerPerson();
                bool petsAllowed = DecideIfPetsAreAllowed();
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

                var response = await _client.UpdateRideAsync(request);
                Console.WriteLine($"Ride update status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error updating ride: {ex.Status.Detail}");
            }
        }

        static async Task DeleteRide()
        {
            Console.Clear();
            Console.WriteLine("Delete Ride");
            try
            {
                // Collect ride ID from the user
                string rideId = SelectRide("Delete");
                if (rideId == null) return; // Return to main menu if no rides are available

                var request = new RideRequest { RideId = rideId };

                var response = await _client.DeleteRideAsync(request);
                Console.WriteLine($"Ride deletion status: {response.Status}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error deleting ride: {ex.Status.Detail}");
            }
        }

        static async Task ViewRideRatings()
        {
            Console.Clear();
            Console.WriteLine("View Ride Ratings");
            try
            {
                // Collect ride ID from the user
                string rideId = SelectRide("View Ratings");
                if (rideId == null) return; // Return to main menu if no rides are available

                var request = new RideRequest { RideId = rideId };

                var response = await _client.GetRideRatingsAsync(request);
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
            try
            {
                // Collect number of rides from the user
                Console.Write("Enter the number of rides to view: ");
                int n = int.Parse(Console.ReadLine());

                var request = new StreamRidesRequest { Count = n };

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

        static void DisplayRide(Ride ride)
        {
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

            if (!response.Rides.Any())
            {
                Console.WriteLine("No rides found.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                return null;
            }

            int startIndex = 0;

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
                    Console.WriteLine($"Ride {startIndex + i + 1}");
                    DisplayRide(ridesToShow[i]);
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
            if (!int.TryParse(Console.ReadLine(), out int rideIndex) || rideIndex <= 0 || rideIndex > response.Rides.Count)
            {
                Console.WriteLine("Invalid ride number selected.");
                return null;
            }

            return response.Rides[rideIndex - 1].RideId;
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
            if (!int.TryParse(Console.ReadLine(), out int cityIndex) || cityIndex <= 0 || cityIndex > cities.Count)
            {
                Console.WriteLine("Invalid city number selected.");
                return SelectCity(cityType);
            }

            return (int)cities[cityIndex - 1];
        }

        static string InputDepartureTime()
        {
            Console.Write("Enter Year of Departure: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Invalid year entered.");
                return InputDepartureTime();
            }

            Console.Write("Enter Month of Departure: ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month entered.");
                return InputDepartureTime();
            }

            Console.Write("Enter Day of Departure: ");
            if (!int.TryParse(Console.ReadLine(), out int day) || day < 1 || day > DateTime.DaysInMonth(year, month))
            {
                Console.WriteLine("Invalid day entered.");
                return InputDepartureTime();
            }

            Console.Write("Enter Hour of Departure (24-hour format): ");
            if (!int.TryParse(Console.ReadLine(), out int hour) || hour < 0 || hour > 23)
            {
                Console.WriteLine("Invalid hour entered.");
                return InputDepartureTime();
            }

            DateTime departureTime = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);
            return departureTime.ToString("o");
        }

        static int PickAmountOfAvailableSeats()
        {
            Console.WriteLine("Introduce the number of seats available on your vehicle");

            Console.WriteLine("1");
            Console.WriteLine("2");
            Console.WriteLine("3");
            Console.WriteLine("4");
            Console.WriteLine("5");
            Console.WriteLine("6");

            string optionSelected = Console.ReadLine();

            if (int.TryParse(optionSelected, out int optionValue))
            {
                if (optionValue > 0 && optionValue <= 6)
                {
                    return optionValue;
                }
            }

            Console.WriteLine("Please introduce a valid number of seats (1-6).");
            return PickAmountOfAvailableSeats();
        }

        static double IntroducePricePerPerson()
        {
            Console.WriteLine("Introduce the price per person of your ride");

            string priceSet = Console.ReadLine();

            if (double.TryParse(priceSet, out var pricePerPerson) && pricePerPerson >= 0)
            {
                return pricePerPerson;
            }

            Console.WriteLine("Please introduce a correct numeric value for the price, try again.");
            return IntroducePricePerPerson();
        }

        static bool DecideIfPetsAreAllowed()
        {
            Console.WriteLine("Do you want to allow pets in your vehicle?");
            Console.WriteLine("Y - If yes");
            Console.WriteLine("Another key - If not");

            string optionSelected = Console.ReadLine().ToUpper();

            if (optionSelected != null && optionSelected.Equals("Y"))
            {
                Console.WriteLine("You have allowed pets on your vehicle");
                return true;
            }
            else
            {
                Console.WriteLine("You have not allowed pets on your vehicle");
                return false;
            }
        }
    }
}
