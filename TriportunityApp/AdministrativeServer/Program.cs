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

                try
                {
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
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
                {
                    Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
                    keepRunning = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }

                if (keepRunning && option != "5")
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
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error creating ride: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        static async Task EditRide()
        {
            Console.Clear();
            Console.WriteLine("Edit Ride");
            try
            {
                string rideId;
                while (true)
                {
                    rideId = SelectRide("Edit");
                    if (rideId != null)
                        break;
                    else
                        Console.WriteLine("Invalid selection. Please try again.");
                }

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
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error updating ride: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }



        static async Task DeleteRide()
        {
            Console.Clear();
            Console.WriteLine("Delete Ride");
            try
            {
                string rideId;
                while (true)
                {
                    rideId = SelectRide("Delete");
                    if (rideId != null)
                        break;
                    else
                        Console.WriteLine("Invalid selection. Please try again.");
                }

                var request = new RideRequest { RideId = rideId };

                var response = await _client.DeleteRideAsync(request);
                Console.WriteLine($"Ride deletion status: {response.Status}");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error deleting ride: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }



        static async Task ViewRideRatings()
        {
            Console.Clear();
            Console.WriteLine("View Ride Ratings");
            try
            {
                string rideId = SelectRide("View Ratings");
                if (rideId == null) return;

                var request = new RideRequest { RideId = rideId };

                var response = await _client.GetRideRatingsAsync(request);
                foreach (var rating in response.Ratings)
                {
                    Console.WriteLine($"Rating: {rating.Punctuation}, Comment: {rating.Comment}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving ride ratings: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        static async Task ViewNextNRides()
        {
            Console.Clear();
            Console.WriteLine("View Next N Rides");
            try
            {
                int n;
                do
                {
                    Console.Write("Enter the number of rides to view : ");
                } while (!int.TryParse(Console.ReadLine(), out n) || n < 1);

                var request = new StreamRidesRequest { Count = n };

                using var call = _client.StreamRides(request);
                int rideCount = 0;
                await foreach (var ride in call.ResponseStream.ReadAllAsync())
                {
                    rideCount++;
                    DisplayRide(ride, rideCount);

                    if (rideCount == n)
                    {
                        break;
                    }
                }

                Console.WriteLine("You have seen the next {0} rides.", n);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                Console.WriteLine("The server is unavailable. It has been shut down. Please try again later.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving next rides: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }



        static void DisplayRide(Ride ride, int index)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine($"Ride {index}:");
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

            while (true)
            {
                var ridesToShow = response.Rides.Skip(startIndex).Take(10).ToList();
                if (!ridesToShow.Any())
                {
                    Console.WriteLine("No more rides to show.");
                    return null; // No rides to show, exit the loop
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

            while (true)
            {
                Console.Write("Enter the number of the ride: ");
                if (int.TryParse(Console.ReadLine(), out int rideIndex) && rideIndex > 0 && rideIndex <= response.Rides.Count)
                {
                    return response.Rides[rideIndex - 1].RideId;
                }
                else
                {
                    Console.WriteLine("Invalid ride number selected. Please try again.");
                }
            }
        }



        static int SelectCity(string cityType)
        {
            while (true)
            {
                Console.WriteLine($"Select {cityType}");
                var cities = System.Enum.GetValues(typeof(CitiesEnum)).Cast<CitiesEnum>().ToList();

                for (int i = 0; i < cities.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {cities[i]}");
                }

                Console.Write("Enter the number of the city: ");
                if (int.TryParse(Console.ReadLine(), out int cityIndex) && cityIndex >= 1 && cityIndex <= cities.Count)
                {
                    return cityIndex - 1;
                }
                else
                {
                    Console.WriteLine("Invalid city number selected. Please try again.");
                }
            }
        }

        static string InputDepartureTime()
        {
            while (true)
            {
                try
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
                catch (Exception)
                {
                    Console.WriteLine("Invalid date or time entered. Please try again.");
                }
            }
        }

        static int PickAmountOfAvailableSeats()
        {
            while (true)
            {
                Console.WriteLine("Introduce the number of seats available on your vehicle");

                Console.WriteLine("1");
                Console.WriteLine("2");
                Console.WriteLine("3");
                Console.WriteLine("4");
                Console.WriteLine("5");
                Console.WriteLine("6");

                if (int.TryParse(Console.ReadLine(), out int optionValue) && optionValue > 0 && optionValue <= 6)
                {
                    return optionValue;
                }
                else
                {
                    Console.WriteLine("Please introduce a valid number of seats (1-6).");
                }
            }
        }

        static double IntroducePricePerPerson()
        {
            while (true)
            {
                Console.WriteLine("Introduce the price per person of your ride");

                if (double.TryParse(Console.ReadLine(), out double pricePerPerson) && pricePerPerson >= 0)
                {
                    return pricePerPerson;
                }
                else
                {
                    Console.WriteLine("Please introduce a correct numeric value for the price, try again.");
                }
            }
        }

        static bool DecideIfPetsAreAllowed()
        {
            while (true)
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
                else if (optionSelected != null && !optionSelected.Equals("Y"))
                {
                    Console.WriteLine("You have not allowed pets on your vehicle");
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }
    }
}
