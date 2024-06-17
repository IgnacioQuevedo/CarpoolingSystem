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
                            await CreateRideAsync();
                            break;
                        case "2":
                            await EditRideAsync();
                            break;
                        case "3":
                            await DeleteRideAsync();
                            break;
                        case "4":
                            await ViewRideRatingsAsync();
                            break;
                        case "5":
                            await ViewNextNRidesAsync();
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

        static async Task CreateRideAsync()
        {
            Console.Clear();
            Console.WriteLine("Create Ride");
            try
            {
                Console.Write("Driver ID: ");
                string driverId = Console.ReadLine();
                int initialLocation = await SelectCityAsync("Initial Location");
                int endingLocation = await SelectCityAsync("Ending Location");

                string departureTime = await InputDepartureTimeAsync();

                int availableSeats = await PickAmountOfAvailableSeatsAsync();
                double pricePerPerson = await IntroducePricePerPersonAsync();
                bool petsAllowed = await DecideIfPetsAreAllowedAsync();
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

        static async Task EditRideAsync()
        {
            Console.Clear();
            Console.WriteLine("Edit Ride");
            try
            {
                string rideId = await SelectRideAsync("Edit");
                if (rideId == null)
                {
                    Console.WriteLine("No ride selected or available. Returning to menu.");
                    return;
                }

                Console.Write("Driver ID: ");
                string driverId = Console.ReadLine();
                int initialLocation = await SelectCityAsync("Initial Location");
                int endingLocation = await SelectCityAsync("Ending Location");

                string departureTime = await InputDepartureTimeAsync();

                int availableSeats = await PickAmountOfAvailableSeatsAsync();
                double pricePerPerson = await IntroducePricePerPersonAsync();
                bool petsAllowed = await DecideIfPetsAreAllowedAsync();
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

        static async Task DeleteRideAsync()
        {
            Console.Clear();
            Console.WriteLine("Delete Ride");
            try
            {
                string rideId = await SelectRideAsync("Delete");
                if (rideId == null)
                {
                    Console.WriteLine("No ride selected or available. Returning to menu.");
                    return;
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

        static async Task ViewRideRatingsAsync()
        {
            Console.Clear();
            Console.WriteLine("View Ride Ratings");
            try
            {
                string rideId = await SelectRideAsync("View Ratings");
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

        static async Task ViewNextNRidesAsync()
        {
            Console.Clear();
            Console.WriteLine("View Next N Rides");
            try
            {
                int n;
                do
                {
                    Console.Write("Enter the number of rides to view: ");
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
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
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

        static async Task<string> SelectRideAsync(string action)
        {
            Console.WriteLine($"Select Ride to {action}");
            var request = new Empty();
            RidesResponse response;

            try
            {
                response = await _client.GetAllRidesAsync(request);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error retrieving rides: {ex.Status.Detail}");
                return null;
            }

            if (response.Rides.Count == 0)
            {
                Console.WriteLine("No rides available.");
                return null;
            }

            int startIndex = 0;
            bool showMore = true;

            while (showMore)
            {
                var ridesToShow = response.Rides.Skip(startIndex).Take(10).ToList();
                if (!ridesToShow.Any())
                {
                    Console.WriteLine("No more rides to show.");
                    return null;
                }

                for (int i = 0; i < ridesToShow.Count; i++)
                {
                    DisplayRide(ridesToShow[i], startIndex + i + 1);
                }

                if (ridesToShow.Count < 10)
                {
                    showMore = false;
                }
                else
                {
                    Console.Write("Show more rides? (Y/N): ");
                    showMore = Console.ReadLine().ToUpper() == "Y";
                    startIndex += 10;
                }
            }

            Console.Write("Enter the number of the ride: ");
            int rideIndex;
            bool validInput = int.TryParse(Console.ReadLine(), out rideIndex);

            while (!validInput || rideIndex <= 0 || rideIndex > response.Rides.Count)
            {
                Console.WriteLine("Invalid ride number selected. Please try again.");
                Console.Write("Enter the number of the ride: ");
                validInput = int.TryParse(Console.ReadLine(), out rideIndex);
            }

            return response.Rides[rideIndex - 1].RideId;
        }

        static async Task<int> SelectCityAsync(string cityType)
        {
            bool validInput;
            int cityIndex;

            do
            {
                Console.WriteLine($"Select {cityType}");
                var cities = System.Enum.GetValues(typeof(CitiesEnum)).Cast<CitiesEnum>().ToList();

                for (int i = 0; i < cities.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {cities[i]}");
                }

                Console.Write("Enter the number of the city: ");
                validInput = int.TryParse(Console.ReadLine(), out cityIndex) && cityIndex >= 1 && cityIndex <= cities.Count;

                if (!validInput)
                {
                    Console.WriteLine("Invalid city number selected. Please try again.");
                }
            } while (!validInput);

            return cityIndex - 1;
        }

        static async Task<string> InputDepartureTimeAsync()
        {
            bool validInput;
            int year, month, day, hour;
            DateTime departureTime = new DateTime();

            do
            {
                try
                {
                    Console.Write("Enter Year of Departure: ");
                    validInput = int.TryParse(Console.ReadLine(), out year);
                    if (!validInput || year < DateTime.Now.Year) throw new Exception();

                    Console.Write("Enter Month of Departure: ");
                    validInput = int.TryParse(Console.ReadLine(), out month);
                    if (!validInput || month < 1 || month > 12) throw new Exception();

                    Console.Write("Enter Day of Departure: ");
                    validInput = int.TryParse(Console.ReadLine(), out day);
                    if (!validInput || day < 1 || day > DateTime.DaysInMonth(year, month)) throw new Exception();

                    Console.Write("Enter Hour of Departure (24-hour format): ");
                    validInput = int.TryParse(Console.ReadLine(), out hour);
                    if (!validInput || hour < 0 || hour > 23) throw new Exception();

                    departureTime = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);
                    validInput = true;
                }
                catch
                {
                    validInput = false;
                    Console.WriteLine("Invalid date or time entered. Please try again.");
                }
            } while (!validInput);

            return departureTime.ToString("o");
        }



        static async Task<int> PickAmountOfAvailableSeatsAsync()
        {
            bool validInput;
            int optionValue;

            do
            {
                Console.WriteLine("Introduce the number of seats available on your vehicle");

                Console.WriteLine("1");
                Console.WriteLine("2");
                Console.WriteLine("3");
                Console.WriteLine("4");
                Console.WriteLine("5");
                Console.WriteLine("6");

                validInput = int.TryParse(Console.ReadLine(), out optionValue) && optionValue > 0 && optionValue <= 6;

                if (!validInput)
                {
                    Console.WriteLine("Please introduce a valid number of seats (1-6).");
                }
            } while (!validInput);

            return optionValue;
        }

        static async Task<double> IntroducePricePerPersonAsync()
        {
            bool validInput;
            double pricePerPerson;

            do
            {
                Console.WriteLine("Introduce the price per person of your ride");

                validInput = double.TryParse(Console.ReadLine(), out pricePerPerson) && pricePerPerson >= 0;

                if (!validInput)
                {
                    Console.WriteLine("Please introduce a correct numeric value for the price, try again.");
                }
            } while (!validInput);

            return pricePerPerson;
        }

        static async Task<bool> DecideIfPetsAreAllowedAsync()
        {
            Console.WriteLine("Do you want to allow pets in your vehicle?");
            Console.WriteLine("Y - If yes");
            Console.WriteLine("Any other key - If not");

            var optionSelected = Console.ReadKey().Key;
            Console.WriteLine(); 

            return optionSelected == ConsoleKey.Y;
        }
    }
}
