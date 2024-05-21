using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Client.Objects.EnumsModels;
using Client.Objects.ReviewModels;
using Client.Objects.RideModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;
using Client.Services;
using Common;

namespace Client
{
    internal class Program
    {
        #region Variables Initialization

        private static UserClient _userLogged;
        private static string _optionSelected;
        private static UserService _userService;
        private static RideService _rideService;

        private static bool _closeApp;
        private static int _amountOfCities = Enum.GetValues(typeof(CitiesEnum)).Length;
        private static int _maxSeatsPerCar = 6;

        private static CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private static CancellationToken _token = _cancellationToken.Token;

        #endregion

        #region Socket

        public static TcpClient client;

        #endregion

        public static async Task Main(string[] args)
        {
            try
            {
                client = NetworkHelper.ConnectWithServer();
                _userService = new UserService();
                _rideService = new RideService();

                Console.WriteLine("Waiting for the server to be ready");
                Console.WriteLine("");

                _closeApp = !await NetworkHelper.IsClientConnectedAsync(client);

                while (!_closeApp)
                {
                    if (_userLogged is null)
                    {
                        MainMenuOptions();

                        _optionSelected = Console.ReadLine();
                        switch (_optionSelected)
                        {
                            case "1":
                                await LoginOptionAsync();
                                break;

                            case "2":
                                await RegisterOptionAsync();
                                break;

                            case "3":
                                AboutUsOption();
                                break;
                            case "4":
                                await CloseAppOptionAsync();
                                break;
                            default:
                                WrongDigitInserted();
                                break;
                        }
                    }

                    else
                    {
                        await PossibleActionsToBeDoneByLoggedUser();
                    }
                }
            }
            catch (Exception exceptionCaught)
            {
                Console.WriteLine(
                    "The communication with the server is closed, try again later. Thanks for using our app");
                _closeApp = true;
            }
        }


        #region Main Menu Options

        private static void MainMenuOptions()
        {
            Console.WriteLine("Digit the number of your query");
            Console.WriteLine("1- Sign In");
            Console.WriteLine("2- Sign Up");
            Console.WriteLine("3- Who are we?");
            Console.WriteLine("4- Exit app");
        }

        private static void WrongDigitInserted()
        {
            Console.WriteLine("");
            Console.WriteLine("Insert a valid digit, please.");
        }

        private static async Task CloseAppOptionAsync()
        {
            try
            {
                Console.WriteLine("");
                Console.WriteLine("Closed App with success!");

                await _userService.CloseAppAsync(client);
                _closeApp = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try again to close the app");
                MainMenuOptions();
            }
        }

        private static void AboutUsOption()
        {
            try
            {
                var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent;

                if (directoryInfo != null)
                {
                    string startDirectory = directoryInfo?.Parent.Parent.FullName;
                    string subrouteOfFile = "AboutUs/CompanyInfo.txt";
                    var fileRoute = Path.Combine(startDirectory, subrouteOfFile);
                    Console.WriteLine(File.ReadAllText(fileRoute));
                }
                else
                {
                    Console.WriteLine("Triportunity is a travel web app");
                }

                Console.WriteLine("Enter any key to go back to the main menu");
                Console.ReadKey();
                Console.ReadLine();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                MainMenuOptions();
            }
        }

        private static async Task RegisterOptionAsync()
        {
            try
            {
                DriverInfoClient driverAspectsOfClient = null;
                Console.WriteLine("Insert your Ci for the registration");
                string ci = Console.ReadLine();

                Console.WriteLine("Your username will be:");
                var usernameRegister = Console.ReadLine();

                Console.WriteLine("Register your password:");
                var passwordRegister = Console.ReadLine();

                Console.WriteLine("Insert the same password as above:");
                var repeatedPassword = Console.ReadLine();

                RegisterUserRequest clientToRegister =
                    new RegisterUserRequest(ci, usernameRegister, passwordRegister, repeatedPassword, null);

                await _userService.RegisterClientAsync(client, clientToRegister);
            }
            catch (Exception exception)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exception);
                Console.WriteLine("Redo all again but without this error: " + exception.Message);
            }
        }

        private static async Task LoginOptionAsync()
        {
            try
            {
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();

                LoginUserRequest loginUserRequest = new LoginUserRequest(username, password);
                _userLogged = await _userService.LoginClientAsync(client, loginUserRequest);
            }

            catch (Exception exception)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exception);
                Console.WriteLine("Redo all again but without this error: " + exception.Message);
            }
        }

        #endregion

        #region Logged Options

        public static async Task PossibleActionsToBeDoneByLoggedUser()
        {
            try
            {
                string _optionSelected = "";
                if (_userLogged.DriverAspects != null)

                {
                    Console.WriteLine("Select 1 - To create a Ride");
                    Console.WriteLine("Select 2 - To join a Ride");
                    Console.WriteLine("Select 3 - To Quit a Ride");
                    Console.WriteLine("Select 4 - To view, edit and delete your created rides");
                    Console.WriteLine("Select 5 - To add a review");
                    Console.WriteLine("Select 6 - To filter rides per price");
                    Console.WriteLine("Select 7 - To close the app");

                    _optionSelected = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Select 1 - If you want to be registered as a driver");
                    Console.WriteLine("Select 2 - To join a Ride");
                    Console.WriteLine("Select 3 - To Quit a Ride");
                    Console.WriteLine("Select 4 - To add a review");
                    Console.WriteLine("Select 5 - To filter rides per price");
                    Console.WriteLine("Select 6 - To close the app");
                    _optionSelected = Console.ReadLine();
                }

                if (!int.TryParse(_optionSelected, out int optionParsed) || optionParsed < 1 || optionParsed > 7)
                {
                    WrongDigitInserted();
                    return;
                }

                switch (optionParsed)
                {
                    case 1:
                        if (_userLogged.DriverAspects != null)
                            await CreateRideAsync();
                        else
                            await CreateDriverAsync(_userLogged.Id);
                        break;

                    case 2:
                        await JoinRideAsync();
                        break;

                    case 3:
                        await QuitRideAsync();
                        break;

                    case 4:
                        if (_userLogged.DriverAspects != null)
                            await ViewYourRidesAsync();
                        else
                            await AddReviewAsync();
                        break;

                    case 5:
                        if (_userLogged.DriverAspects != null)
                            await AddReviewAsync();
                        else
                        {
                            await GetRidesByPriceAsync();
                        }

                        break;
                    case 6:
                        if (_userLogged.DriverAspects != null)
                        {
                            await GetRidesByPriceAsync();
                        }
                        else
                        {
                            await CloseAppOptionAsync();
                        }

                        break;
                    case 7:
                        if (_userLogged.DriverAspects != null)
                            await CloseAppOptionAsync();
                        break;
                    default:
                        WrongDigitInserted();
                        break;
                }
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message, exceptionCaught);
            }
        }

        #endregion

        #region Driver creation

        private static async Task CreateDriverAsync(Guid userRegisteredId)
        {
            try
            {
                string carModel;
                string path;
                string addNewVehicle = "Y";

                _userLogged = await _userService.GetUserByIdAsync(client, userRegisteredId);
                bool firstVehicle = _userLogged.DriverAspects == null || _userLogged.DriverAspects.Vehicles.Count == 0;

                while (addNewVehicle.Equals("Y"))
                {
                    Console.WriteLine("Please enter the model of the vehicle");
                    carModel = Console.ReadLine();
                    Console.WriteLine("Please enter the path of the vehicle image");
                    path = Console.ReadLine();

                    if (firstVehicle)
                    {
                        await _userService.CreateDriverAsync(client, userRegisteredId, carModel, path, _token);
                        firstVehicle = false;
                    }
                    else
                    {
                        await _userService.AddVehicleAsync(client, userRegisteredId, carModel, path, _token);
                    }

                    Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                    Console.WriteLine("If yes - Enter 'Y'");
                    Console.WriteLine("If not - Enter any other key");
                    addNewVehicle = Console.ReadLine().ToUpper();
                }

                _userLogged = await _userService.GetUserByIdAsync(client, userRegisteredId);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine("");
                Console.WriteLine(exceptionCaught.Message);
                await CreateDriverAsync(userRegisteredId);
            }
        }

        #endregion

        #region Create Ride

        private static async Task CreateRideAsync()
        {
            try
            {
                Console.WriteLine(
                    "You will have to complete the following steps to have your ride created. Let's start with the creation of your ride!");

                List<Guid> passengers = new List<Guid>();

                Guid vehicleIdSelected = Guid.Parse(await PickVehicleAsync());

                string locationMode = "initial";
                CitiesEnum initialLocation = PickLocation(locationMode);

                locationMode = "ending";
                CitiesEnum endingLocation = PickLocation(locationMode);

                DateTime departureDate = PickDepartureDate();

                int availableSeats = PickAmountOfAvailableSeats();

                double pricePerPerson = IntroducePricePerPerson();

                bool petsAllowed = DecideIfPetsAreAllowed();


                CreateRideRequest rideReq = new CreateRideRequest(_userLogged.Id, passengers,
                    initialLocation, endingLocation,
                    departureDate, availableSeats, pricePerPerson, petsAllowed, vehicleIdSelected);

                await _rideService.CreateRideAsync(client, rideReq);
                Console.WriteLine("Ride created successfully");
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine("Error : " + exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static bool DecideIfPetsAreAllowed()
        {
            bool petsAllowed = true;

            Console.WriteLine("Do you want to allow pets in your vehicle?");
            Console.WriteLine("Y - If yes");
            Console.WriteLine("Another key - If not");

            _optionSelected = Console.ReadLine().ToUpper();

            if (_optionSelected != null && _optionSelected.Equals("Y"))
            {
                Console.WriteLine("You have allowed pets on your vehicle");
            }

            else
            {
                Console.WriteLine("You have not allowed pets on your vehicle");
                petsAllowed = false;
            }

            return petsAllowed;
        }

        private static double IntroducePricePerPerson()
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

        private static async Task<string> PickVehicleAsync()
        {
            Console.WriteLine("Select the vehicle you will use for this ride");

            ICollection<VehicleClient> vehicles = await _userService.GetVehiclesByUserIdAsync(client, _userLogged.Id);

            for (int i = 0; i < vehicles.Count; i++)
            {
                Console.WriteLine(i + " - " + "Vehicle Model: " + vehicles.ElementAt(i).CarModel);
            }

            _optionSelected = Console.ReadLine();

            if (int.TryParse(_optionSelected, out int optionValue) && optionValue >= 0 && optionValue < vehicles.Count)
            {
                Console.WriteLine(vehicles.ElementAt(optionValue).CarModel + " has been selected");
                return vehicles.ElementAt(optionValue).Id.ToString();
            }

            Console.WriteLine("Please introduce a valid digit for the vehicle");
            return await PickVehicleAsync();
        }

        private static int PickAmountOfAvailableSeats()
        {
            Console.WriteLine("Introduce the number of seats available on your vehicle");

            Console.WriteLine("1");
            Console.WriteLine("2");
            Console.WriteLine("3");
            Console.WriteLine("4");
            Console.WriteLine("5");
            Console.WriteLine("6");

            _optionSelected = Console.ReadLine();

            if (int.TryParse(_optionSelected, out int optionValue))
            {
                if (optionValue <= _maxSeatsPerCar && optionValue > 0)
                {
                    return optionValue;
                }

                Console.WriteLine("Please introduce valid numeric values, try again.");
                return PickAmountOfAvailableSeats();
            }

            Console.WriteLine("Please introduce valid numeric values, try again.");
            return PickAmountOfAvailableSeats();
        }

        private static DateTime PickDepartureDate()
        {
            Console.WriteLine("Pick the departure date of the ride");

            Console.WriteLine("Introduce a year");
            string departureYear = Console.ReadLine();

            Console.WriteLine("Introduce the month");
            string departureMonth = Console.ReadLine();

            Console.WriteLine("Introduce the day");
            string departureDay = Console.ReadLine();

            Console.WriteLine("Introduce the hour of departure");
            string departureHour = Console.ReadLine();

            DateTime rideDate = ParseInputsToDate(departureYear, departureMonth, departureDay, departureHour);

            if (rideDate.Date < DateTime.Now.Date)
            {
                Console.WriteLine("The date of the ride must be in the future");
                return PickDepartureDate();
            }

            Console.WriteLine("Departure date selected: " + rideDate.ToString("yyyy-MM-dd"));
            return rideDate;
        }

        private static DateTime ParseInputsToDate(string departureYear, string departureMonth, string departureDay,
            string departureHour)
        {
            if (int.TryParse(departureYear, out int year) &&
                int.TryParse(departureMonth, out int month) &&
                int.TryParse(departureDay, out int day) &&
                int.TryParse(departureHour, out int hour))
            {
                try
                {
                    DateTime departureDate = new DateTime(year, month, day, hour, 0, 0);
                    return departureDate;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Not valid date, introduce again the date");
                    return PickDepartureDate();
                }
            }
            else
            {
                Console.WriteLine("Please use numeric values for year, month and day");
                return PickDepartureDate();
            }
        }

        private static CitiesEnum PickLocation(string locationMode)
        {
            Console.WriteLine($"Select the {locationMode} location of your ride");
            Console.WriteLine();

            ShowCities();
            _optionSelected = Console.ReadLine();
            return PossibleCasesWhenPickingLocation(_optionSelected, locationMode);
        }

        private static void ShowCities()
        {
            for (int i = 1; i <= _amountOfCities; i++)
            {
                string cityName = Enum.GetName(typeof(CitiesEnum), i);
                Console.WriteLine($"Select {i}- {cityName}");
            }
        }

        private static CitiesEnum PossibleCasesWhenPickingLocation(string optionSelected, string locationMode)
        {
            try
            {
                if (int.TryParse(optionSelected, out int optionValue) && optionValue <= _amountOfCities &&
                    optionValue > 0)
                {
                    string cityName = Enum.GetName(typeof(CitiesEnum), optionValue);
                    Console.WriteLine($"You have selected {cityName} as your {locationMode} location");

                    return (CitiesEnum)optionValue;
                }

                Console.WriteLine("You have introduced incorrect values, try again.");
                return PickLocation(locationMode);
            }
            catch (FormatException)
            {
                Console.WriteLine("You have introduced incorrect values, try again.");
                return PickLocation(locationMode);
            }
        }

        #endregion

        #region Join Ride

        private static async Task JoinRideAsync()
        {
            try
            {
                ICollection<RideClient> rides = await _rideService.GetAllRidesAsync(client);
                RideClient selectedRide = await SelectRideFromListAsync(rides.ToList());

                JoinRideRequest joinReq = new JoinRideRequest(selectedRide.Id, _userLogged.Id);

                await _rideService.JoinRideAsync(client, joinReq);
                Console.WriteLine("Join Successfully");
            }
            catch (Exception exceptionCaught)

            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static async Task<RideClient> SelectRideFromListAsync(List<RideClient> rides)
        {
            try
            {
                Console.WriteLine("Select the ride you are aiming into");
                Console.WriteLine();

                DisplayAllRides(rides);

                _optionSelected = Console.ReadLine();

                if (int.TryParse(_optionSelected, out int optionValue))
                {
                    if (optionValue < rides.Count && optionValue >= 0)
                    {
                        RideClient rideSelected = rides[optionValue];
                        Console.WriteLine(
                            $"You have selected the ride From: {rideSelected.InitialLocation} To: {rideSelected.EndingLocation}");
                        Console.WriteLine($"Departure time on: {rideSelected.DepartureTime.ToShortDateString()}");
                        Console.WriteLine($"Price: {rideSelected.PricePerPerson}");

                        Console.WriteLine("Do you want to see the details of the ride? - 'Y' for Yes " +
                                          "or Any other key for No");

                        string seeDetails = Console.ReadLine().ToUpper();

                        if (seeDetails.Equals("Y"))
                        {
                            Console.WriteLine(
                                "And at the moment are available " + rideSelected.AvailableSeats + "seats");
                            Console.WriteLine($"Pets allowed: {rideSelected.PetsAllowed}");

                            Console.WriteLine("Do you want to see the car image?");
                            Console.WriteLine("Y - If yes or Any other key for No");

                            seeDetails = Console.ReadLine().ToUpper();
                            if (seeDetails.Equals("Y"))
                            {
                                Console.WriteLine(@"Your Image is Allocated At: " +
                                                  await _rideService.GetCarImageByIdAsync(client, _userLogged.Id,
                                                      rideSelected.Id, _token));
                            }
                        }

                        return rideSelected;
                    }
                }

                Console.WriteLine("Introduce a valid number");
                return await SelectRideFromListAsync(rides);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                return await SelectRideFromListAsync(rides);
            }
        }


        private static void DisplayAllRides(List<RideClient> rides)
        {
            try
            {
                int amountOfRides = rides.Count;

                RideClient actualRide = new RideClient();

                for (int i = 0; i < amountOfRides; i++)
                {
                    actualRide = rides[i];
                    Console.WriteLine(
                        $" {i} - From: {actualRide.InitialLocation} To: {actualRide.EndingLocation} With date of departure: " +
                        $"{actualRide.DepartureTime.ToShortDateString()} and with a Price per person of : ${actualRide.PricePerPerson}");
                }
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                DisplayAllRides(rides);
            }
        }

        #endregion

        #region View Your Rides

        private static async Task ViewYourRidesAsync()
        {
            try
            {
                ICollection<RideClient> rideCollectionOfUser =
                    await _rideService.GetRidesByUserAsync(client, _userLogged.Id);
                List<RideClient> rideListOfUser = rideCollectionOfUser.ToList();

                DisplayAllRides(rideListOfUser.ToList());

                Console.WriteLine("Select the ride you want to edit: ");
                _optionSelected = Console.ReadLine();

                if (int.TryParse(_optionSelected, out int optionValue)
                    && optionValue >= 0 && optionValue < rideListOfUser.Count)
                {
                    RideClient rideSelected = rideListOfUser[optionValue];
                    Console.WriteLine(
                        $"You have selected the ride From: {rideSelected.InitialLocation} To: {rideSelected.EndingLocation}");
                    Console.WriteLine(
                        $"Departure date on: {rideSelected.DepartureTime.ToShortDateString() + " At: " + rideSelected.DepartureTime.ToLongTimeString()}");
                    Console.WriteLine($"Price: {rideSelected.PricePerPerson}");
                    Console.WriteLine("");

                    Console.WriteLine("Select the respective action do you want to do with this ride: ");
                    Console.WriteLine("1- Modify Ride");
                    Console.WriteLine("2- Delete Ride");
                    Console.WriteLine("3- Disable Ride");
                    Console.WriteLine("4- Get Ride Info");
                    Console.WriteLine("5- Go back to the main menu");

                    await ViewFunctionsAsync(rideSelected);
                }
                else
                {
                    Console.WriteLine("Please introduce a valid digit");
                    await ViewYourRidesAsync();
                }
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static async Task ViewFunctionsAsync(RideClient rideSelected)
        {
            _optionSelected = Console.ReadLine();
            if (int.TryParse(_optionSelected, out int optionToDo) && optionToDo >= 1 && optionToDo <= 5)
            {
                switch (optionToDo)
                {
                    case 1:
                        await ModifyRideAsync(rideSelected);
                        break;
                    case 2:
                        await DeleteRideAsync(rideSelected);
                        break;
                    case 3:
                        await DisableRideAsync(rideSelected);
                        break;
                    case 4:
                        await GetRideInfoAsync(rideSelected);
                        break;
                    case 5:
                        await PossibleActionsToBeDoneByLoggedUser();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please introduce a valid digit");
                await ViewFunctionsAsync(rideSelected);
            }
        }

        #endregion

        #region Modify Ride

        public static async Task ModifyRideAsync(RideClient rideSelected)
        {
            try
            {
                Console.WriteLine("You will have to complete the following steps to have your ride edited");

                string locationMode = "initial";
                CitiesEnum initialLocation = PickLocation(locationMode);

                locationMode = "ending";
                CitiesEnum endingLocation = PickLocation(locationMode);

                DateTime departureDate = PickDepartureDate();

                int availableSeats = PickAmountOfAvailableSeats();

                double pricePerPerson = IntroducePricePerPerson();

                bool petsAllowed = DecideIfPetsAreAllowed();

                Guid vehicleId = Guid.Parse(await PickVehicleAsync());


                ModifyRideRequest modifyRideReq = new ModifyRideRequest(rideSelected.Id,
                    initialLocation, endingLocation, departureDate,
                    pricePerPerson, petsAllowed, vehicleId, availableSeats, rideSelected.DriverId);

                await _rideService.ModifyRideAsync(client, modifyRideReq);
                Console.WriteLine("Ride modified successfully");
            }

            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Quit Ride

        private static async Task QuitRideAsync()
        {
            try
            {
                ICollection<RideClient> rides = await _rideService.GetRidesByUserAsync(client, _userLogged.Id);

                RideClient rideSelected = await SelectRideFromListAsync(rides.ToList());

                QuitRideRequest quitRideReq = new QuitRideRequest(rideSelected.Id, _userLogged);

                await _rideService.QuitRideAsync(client, quitRideReq);
                Console.WriteLine("Quit from ride successfully");
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Delete Ride

        private static async Task DeleteRideAsync(RideClient rideSelected)
        {
            try
            {
                await _rideService.DeleteRideAsync(client, rideSelected.Id);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Disable Ride

        private static async Task DisableRideAsync(RideClient rideSelected)
        {
            try
            {
                await _rideService.DisableRideAsync(client, rideSelected.Id);

                Console.WriteLine("Ride has been disabled");
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Get Ride Info

        private static async Task GetRideInfoAsync(RideClient rideSelected)
        {
            try
            {
                RideClient rideData = await _rideService.GetRideByIdAsync(client, rideSelected.Id);

                Console.WriteLine("This is the information related to the ride you have selected: ");
                Console.WriteLine();

                await _rideService.GetRideByIdAsync(client, rideSelected.Id);

                await DisplayRideAsync(rideSelected);

                Console.WriteLine("Do you want to see the car image?");
                Console.WriteLine("Y - If yes or Any other key for No");

                string seeDetails = Console.ReadLine().ToUpper();

                if (seeDetails == "Y")
                {
                    await _rideService.GetCarImageByIdAsync(client, _userLogged.Id, rideData.Id, _token);
                }
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static async Task DisplayRideAsync(RideClient ride)
        {
            Console.WriteLine($"\nInitial Location: {ride.InitialLocation}");
            Console.WriteLine($"Ending Location: {ride.EndingLocation}");
            Console.WriteLine($"Departure Time: {ride.DepartureTime}");
            Console.WriteLine($"Available Seats: {ride.AvailableSeats}");
            Console.WriteLine($"Price Per Person: {ride.PricePerPerson}");
            Console.WriteLine($"Pets Allowed: {ride.PetsAllowed}");
            Console.WriteLine($"Vehicle ID: {ride.VehicleId}");
            Console.WriteLine("");
            await GetDriverReviewsAsync(ride.Id);
            Console.WriteLine("");
        }

        #endregion

        #region Get Rides By Price

        private static async Task GetRidesByPriceAsync()
        {
            try
            {
                Console.WriteLine("\nIntroduce the minimum price :");
                string minPriceInput = Console.ReadLine();
                double minPrice;

                while (!double.TryParse(minPriceInput, out minPrice))
                {
                    Console.WriteLine("Invalid input for minimum price. Please enter a valid number.");
                    minPriceInput = Console.ReadLine();
                }

                minPrice = Double.Parse(minPriceInput);

                Console.WriteLine("\nIntroduce the maximum price :");
                string maxPriceInput = Console.ReadLine();
                double maxPrice;

                while (!double.TryParse(maxPriceInput, out maxPrice))
                {
                    Console.WriteLine("Invalid input for maximum price. Please enter a valid number.");
                    maxPriceInput = Console.ReadLine();
                }

                maxPrice = Double.Parse(maxPriceInput);

                ICollection<RideClient> rides =
                    await _rideService.GetRidesFilteredByPriceAsync(client, minPrice, maxPrice);

                Console.WriteLine("\nRides with price between " + minPrice + " and " + maxPrice + " are: ");
                DisplayAllRides(rides.ToList());
                Console.WriteLine("");
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                Console.WriteLine("");
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Add Review

        public static async Task AddReviewAsync()
        {
            try
            {
                ICollection<RideClient> rides = await _rideService.GetRidesByUserAsync(client, _userLogged.Id);
                RideClient rideClient = await SelectRideFromListAsync(rides.ToList());


                Console.WriteLine("\nIntroduce the rating you want to give to the driver");
                string ratingInput = Console.ReadLine();
                double rating;

                while (!double.TryParse(ratingInput, out rating))
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 0.0 and 5.0.");
                    ratingInput = Console.ReadLine();
                }

                rating = Double.Parse(ratingInput);

                Console.WriteLine("Introduce the comment you want to give to the driver");
                string comment = Console.ReadLine();

                ReviewClient reviewRequest = new ReviewClient(rideClient.Id, rating, comment);
                await _rideService.AddReviewAsync(client, _userLogged.Id, reviewRequest);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                Console.WriteLine("");
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region GetDriverReview

        public static async Task GetDriverReviewsAsync(Guid rideId)
        {
            try
            {
                ICollection<ReviewClient> reviews = await _rideService.GetDriverReviews(client, rideId);

                List<ReviewClient> reviewsList = new List<ReviewClient>(reviews);

                DisplayAllReviews(reviewsList);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                Console.WriteLine(exceptionCaught.Message);
                Console.WriteLine("");
                await PossibleActionsToBeDoneByLoggedUser();
            }
        }

        public static void DisplayAllReviews(List<ReviewClient> reviews)
        {
            int amountOfReviews = reviews.Count;

            ReviewClient actualReview = new ReviewClient();

            for (int i = 0; i < amountOfReviews; i++)
            {
                actualReview = reviews[i];
                Console.WriteLine(
                    $" {i} -  Driver ID: {actualReview.DriverId}  - Rating : {actualReview.Punctuation} - Comment : {actualReview.Comment}");
            }
        }

        #endregion
    }
}