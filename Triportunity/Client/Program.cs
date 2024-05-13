using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

        #endregion

        #region Socket

        public static Socket clientSocket;

        #endregion

        public static void Main(string[] args)
        {
            clientSocket = NetworkHelper.ConnectWithServer();
            _userService = new UserService(clientSocket);
            _rideService = new RideService(clientSocket);

            Console.WriteLine("Waiting for the server to be ready");
            Console.WriteLine("");

            _closeApp = !NetworkHelper.IsSocketConnected(clientSocket);

            while (!_closeApp)
            {
                if (_userLogged is null)
                {
                    MainMenuOptions();

                    _optionSelected = Console.ReadLine();
                    switch (_optionSelected)
                    {
                        case "1":
                            LoginOption();
                            break;

                        case "2":
                            RegisterOption();
                            break;

                        case "3":
                            AboutUsOption();
                            break;
                        case "4":
                            CloseAppOption();
                            break;
                        default:
                            WrongDigitInserted();
                            break;
                    }
                }

                else
                {
                    PossibleActionsToBeDoneByLoggedUser();
                }
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

        private static void CloseAppOption()
        {
            try
            {
                Console.WriteLine("");
                Console.WriteLine("Closed App with success!");

                _userService.CloseApp(clientSocket);
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

        private static void RegisterOption()
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

                _userService.RegisterClient(clientSocket, clientToRegister);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Redo all again but without this error: " + exception.Message);
            }
        }

        private static void LoginOption()
        {
            try
            {
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();

                LoginUserRequest loginUserRequest = new LoginUserRequest(username, password);
                _userLogged = _userService.LoginClient(clientSocket, loginUserRequest);
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.WriteLine(exception.Message);
            }
        }

        #endregion

        #region Logged Options

        public static void PossibleActionsToBeDoneByLoggedUser()
        {
            string _optionSelected = "";
            if (_userLogged.DriverAspects != null)

            {
                Console.WriteLine("Select 1 - To create a Ride");
                Console.WriteLine("Select 2 - To join a Ride");
                Console.WriteLine("Select 3 - To Quit a Ride");
                Console.WriteLine("Select 4 - To view, edit and delete your created rides");
                Console.WriteLine("Select 5 - To add a review");
                Console.WriteLine("Select 6 - To filter rides per price, starting location or ending location");
                Console.WriteLine("Select 7 - To close the app");

                _optionSelected = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Select 1 - If you want to be registered as a driver");
                Console.WriteLine("Select 2 - To join a Ride");
                Console.WriteLine("Select 3 - To Quit a Ride");
                Console.WriteLine("Select 4 - To add a review");
                Console.WriteLine("Select 5 - To filter rides per price, starting location or ending location");
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
                        CreateRide();
                    else
                        CreateDriver(_userLogged.Id);
                    break;

                case 2:
                    JoinRide();
                    break;

                case 3:
                    QuitRide();
                    break;

                case 4:
                    if (_userLogged.DriverAspects != null)
                        ViewYourRides();
                    else
                        AddReview();
                    break;

                case 5:
                    if (_userLogged.DriverAspects != null)
                        AddReview();
                    else
                    {
                        FilterRides();
                    }

                    break;
                case 6:
                    if (_userLogged.DriverAspects != null) {
                        FilterRides();
                        }
                    else
                    {
                        CloseAppOption();
                    }
                    break;
                case 7:
                    if(_userLogged.DriverAspects != null)
                    CloseAppOption();
                    break;
                default:
                    WrongDigitInserted();
                    break;
            }
        }

        #endregion


        #region Filter Rides

        private static void FilterRides()
        {
            try
            {
                Console.WriteLine("Select the filter you want to apply to the rides");
                Console.WriteLine("1- Filter by price");
                Console.WriteLine("2- Filter by initial location");
                Console.WriteLine("3- Filter by ending location");

                _optionSelected = Console.ReadLine();

                if (int.TryParse(_optionSelected, out int optionValue) && optionValue >= 1 && optionValue <= 3)
                {
                    switch (optionValue)
                    {
                        case 1:
                            GetRidesByPrice();
                            break;
                        case 2:
                            GetRidesByInitialLocation();
                            break;
                        case 3:
                            GetRidesByEndingLocation();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Please introduce a valid digit");
                    FilterRides();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                PossibleActionsToBeDoneByLoggedUser();
            }
        }



        #endregion

        #region Driver creation

        private static void CreateDriver(Guid userRegisteredId)
        {
            try
            {
                string carModel;
                string path;
                string addNewVehicle = "Y";
                
                _userLogged = _userService.GetUserById(clientSocket, userRegisteredId);
                bool firstVehicle = _userLogged.DriverAspects == null || _userLogged.DriverAspects.Vehicles.Count == 0;

                while (addNewVehicle.Equals("Y"))
                {
                    Console.WriteLine("Please enter the model of the vehicle");
                    carModel = Console.ReadLine();
                    Console.WriteLine("Please enter the path of the vehicle image");
                    path = Console.ReadLine();

                    if (firstVehicle)
                    {
                        _userService.CreateDriver(clientSocket, userRegisteredId, carModel, path);
                        firstVehicle = false;
                       
                    }
                    else
                    {
                        _userService.AddVehicle(clientSocket, userRegisteredId, carModel, path);
                    }
                    Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                    Console.WriteLine("If yes - Enter 'Y'");
                    Console.WriteLine("If not - Enter any other key");
                    addNewVehicle = Console.ReadLine();
                }
                _userLogged = _userService.GetUserById(clientSocket, userRegisteredId);
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(e.Message);
                CreateDriver(userRegisteredId);
            }
        }

        #endregion


        #region Create Ride

        private static void CreateRide()
        {
            try
            {
                Console.WriteLine(
                    "You will have to complete the following steps to have your ride created. Let's start with the creation of your ride!");

                List<Guid> passengers = new List<Guid>();

                Guid vehicleIdSelected = Guid.Parse(PickVehicle());

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

                _rideService.CreateRide(rideReq);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static bool DecideIfPetsAreAllowed()
        {
            bool result = false;

            Console.WriteLine("Do you want to allow pets in your vehicle?");
            Console.WriteLine("1- If yes");
            Console.WriteLine("2- If not");

            _optionSelected = Console.ReadLine();

            if (_optionSelected == "1")
            {
                Console.WriteLine("You have allowed pets on your vehicle");
                result = true;
            }
            else if (_optionSelected == "2")
            {
                Console.WriteLine("You have not allowed pets on your vehicle");
                result = false;
            }

            return result;
        }

        private static double IntroducePricePerPerson()
        {
            double pricePerPerson;
            Console.WriteLine("Introduce the price per person of your ride");

            string priceSetted = Console.ReadLine();

            if (double.TryParse(priceSetted, out pricePerPerson))
            {
                return pricePerPerson;
            }
            else
            {
                Console.WriteLine("Please introduce a numeric value for the price, try again...");
                return pricePerPerson;
            }
        }

        private static string PickVehicle()
        {
            Console.WriteLine("Select the vehicle you will use for this ride");

            ICollection<VehicleClient> vehicles = _userService.GetVehiclesByUserId(_userLogged.Id);

            foreach (var vehicle in vehicles)
            {
                Console.WriteLine("Vehicle Id: " + vehicle.Id);
                Console.WriteLine("Vehicle Model: " + vehicle.CarModel);
            }

            string vehicleIdSelected = Console.ReadLine();

            return vehicleIdSelected;
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
                if (optionValue <= _maxSeatsPerCar)
                {
                    return optionValue;
                }
                else
                {
                    Console.WriteLine("Please introduce valid numeric values, try again...");
                    return PickAmountOfAvailableSeats();
                }
            }
            else
            {
                Console.WriteLine("Please introduce valid numeric values, try again...");
                return PickAmountOfAvailableSeats();
            }
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

            return ParseInputsToDate(departureYear, departureMonth, departureDay, departureHour);
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
                    Console.WriteLine("Departure date selected: " + departureDate.ToString("yyyy-MM-dd"));
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
                int optionValue = int.Parse(_optionSelected);

                if (optionValue <= _amountOfCities)
                {
                    string cityName = Enum.GetName(typeof(CitiesEnum), optionValue);
                    Console.WriteLine($"You have selected {cityName} as your initial location");

                    return (CitiesEnum)optionValue;
                }
                else
                {
                    Console.WriteLine("You have introduced incorrect values, try again...");
                    return PickLocation(locationMode);
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("You have introduced incorrect values, try again...");
                return PickLocation(locationMode);
            }
        }

        #endregion

        #region Join Ride

        private static void JoinRide()
        {
            try
            {
                ICollection<RideClient> rides = _rideService.GetAllRides();
                RideClient selectedRide = SelectRideFromList(rides.ToList());


                JoinRideRequest joinReq = new JoinRideRequest(selectedRide.Id, _userLogged.Id);

                _rideService.JoinRide(joinReq);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }
        private static RideClient SelectRideFromList(List<RideClient> rides)
        {
            try
            {
                Console.WriteLine("Select the ride you are aiming into");
                Console.WriteLine();

                DisplayAllRides(rides);

                _optionSelected = Console.ReadLine();

                string[] parts = _optionSelected.Split('-');

                int optionValue;

                if (int.TryParse(parts[0].Trim(), out optionValue))
                {
                    if (optionValue <= rides.Count)
                    {
                        RideClient rideSelected = rides[optionValue];
                        Console.WriteLine(
                            $"You have selected the ride From: {rideSelected.InitialLocation} To: {rideSelected.EndingLocation}");
                        Console.WriteLine($"Departure time on: {rideSelected.DepartureTime.ToShortDateString()}");
                        Console.WriteLine($"Price: {rideSelected.PricePerPerson}");

                        Console.WriteLine("Do you want to see the details of the ride? -- 'Y' for Yes or 'N' for No");
                        string seeDetails = Console.ReadLine();
                        if (seeDetails == "Y")
                        {
                            Console.WriteLine(
                                "And at the moment are available " + rideSelected.AvailableSeats + "seats");
                            Console.WriteLine($"Pets allowed: {rideSelected.PetsAllowed}");

                            Console.WriteLine("Do you want to see the car image?");
                            seeDetails = Console.ReadLine();
                            if (seeDetails == "Y")
                            {
                                Console.WriteLine(@"Your Image is Allocated At: " +
                                                  _rideService.GetCarImageById(_userLogged.Id, rideSelected.Id));
                            }
                        }

                        return rideSelected;
                    }

                    Console.WriteLine("You must introduce a valid digit for the ride");
                    return SelectRideFromList(rides);
                }

                Console.WriteLine("Introduce a valid number");
                return SelectRideFromList(rides);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return SelectRideFromList(rides);
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DisplayAllRides(rides);
            }
        }

        #endregion

        private static void ViewYourRides()
        {
            try
            {
                int index = 0;
                List<RideClient> rideListOfUser = _rideService.GetRidesByUser(_userLogged.Id).ToList();

                DisplayAllRides(rideListOfUser.ToList());

                Console.WriteLine("Select the ride you want to edit: ");
                _optionSelected = Console.ReadLine();
                string[] parts = _optionSelected.Split('-');

                int optionValue;

                if (int.TryParse(parts[0].Trim(), out optionValue))
                {
                    if (optionValue <= rideListOfUser.Count)
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

                        _optionSelected = Console.ReadLine();
                        if (int.TryParse(_optionSelected, out int optionToDo) && optionToDo >= 1 && optionToDo <= 5)
                        {
                            switch (optionToDo)
                            {
                                case 1:
                                    ModifyRide(rideSelected);
                                    break;
                                case 2:
                                    DeleteRide(rideSelected);
                                    break;
                                case 3:
                                    DisableRide(rideSelected);
                                    break;
                                case 4:
                                    GetRideInfo(rideSelected);
                                    break;
                                case 5:
                                    PossibleActionsToBeDoneByLoggedUser();
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please introduce a valid digit");
                            ViewYourRides();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }


        #region Modify Ride

        public static void ModifyRide(RideClient rideSelected)
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

                Guid vehicleId = Guid.Parse(PickVehicle());


                ModifyRideRequest modifyRideReq = new ModifyRideRequest(rideSelected.Id,
                    rideSelected.Passengers,
                    initialLocation, endingLocation, departureDate,
                    pricePerPerson, petsAllowed, vehicleId, availableSeats, rideSelected.DriverId);

                _rideService.ModifyRide(modifyRideReq);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Quit Ride

        private static void QuitRide()
        {
            try
            {
                ICollection<RideClient> ridesCollection = _rideService.GetRidesByUser(_userLogged.Id);
                List<RideClient> ridesList = new List<RideClient>(ridesCollection);

                RideClient rideSelected = SelectRideFromList(ridesList);

                QuitRideRequest quitRideReq = new QuitRideRequest(rideSelected.Id, _userLogged);

                _rideService.QuitRide(quitRideReq);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Delete Ride

        private static void DeleteRide(RideClient rideSelected)
        {
            try
            {
                _rideService.DeleteRide(rideSelected.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Disable Ride

        private static void DisableRide(RideClient rideSelected)
        {
            try
            {
                _rideService.DisableRide(rideSelected.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Get Ride Info

        private static void GetRideInfo(RideClient rideSelected)
        {
            try
            {

                RideClient rideData = _rideService.GetRideById(rideSelected.Id);

                Console.WriteLine("This is the information that related to the ride you have selected: ");
                Console.WriteLine();
                Console.WriteLine(
                    $"From: {rideData.InitialLocation} To: {rideData.EndingLocation} With date of departure: " +
                    $"{rideData.DepartureTime.ToShortDateString()} and with a Price per person of : ${rideData.PricePerPerson}");

                Console.WriteLine("And at the moment are available " + rideData.AvailableSeats + "seats");

                Console.WriteLine($"Pets allowed: {rideData.PetsAllowed}");

                Console.WriteLine("Do you want to see the car image?");

                string seeDetails = Console.ReadLine();

                if (seeDetails == "Y")
                {
                    _rideService.GetCarImageById(_userLogged.Id, rideData.Id);
                }

                //_rideService.GetRideById(rideSelected.Id);

                //DisplayRide(rideSelected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        private static void DisplayRide(RideClient ride)
        {
            Console.WriteLine($"\nRide ID: {ride.Id}");
            Console.WriteLine($"Driver ID: {ride.DriverId}");
            Console.WriteLine($"Initial Location: {ride.InitialLocation}");
            Console.WriteLine($"Ending Location: {ride.EndingLocation}");
            Console.WriteLine($"Departure Time: {ride.DepartureTime}");
            Console.WriteLine($"Available Seats: {ride.AvailableSeats}");
            Console.WriteLine($"Price Per Person: {ride.PricePerPerson}");
            Console.WriteLine($"Pets Allowed: {ride.PetsAllowed}");
            Console.WriteLine($"Vehicle ID: {ride.VehicleId}");
            Console.WriteLine("");
            GetDriverReviews(ride.Id);
            Console.WriteLine("");
        }

        #endregion

        #region Get Rides By Price

        private static void GetRidesByPrice()
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

                ICollection<RideClient> rides = _rideService.GetRidesFilteredByPrice(minPrice, maxPrice);

                Console.WriteLine("\nRides with price between " + minPrice + " and " + maxPrice + " are: ");
                DisplayAllRides(rides.ToList());
                Console.WriteLine("");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                PossibleActionsToBeDoneByLoggedUser();
            }
        }


        #endregion

        #region Get Rides By Initial Location

        private static void GetRidesByInitialLocation()
        {
            try
            {
                Console.WriteLine("Introduce the initial location you want to filter the rides by");

                ShowCities();

                _optionSelected = Console.ReadLine();

                CitiesEnum initialLocation = PossibleCasesWhenPickingLocation(_optionSelected, "initial");

                _rideService.GetRidesFilteredByInitialLocation(initialLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Get Rides By Ending Location

        private static void GetRidesByEndingLocation()
        {
            try
            {
                Console.WriteLine("Introduce the ending location you want to filter the rides by");

                ShowCities();

                _optionSelected = Console.ReadLine();

                CitiesEnum endingLocation = PossibleCasesWhenPickingLocation(_optionSelected, "ending");

                _rideService.GetRidesFilteredByEndingLocation(endingLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PossibleActionsToBeDoneByLoggedUser();
            }
        }

        #endregion

        #region Add Review

        public static void AddReview()
        {
            try
            {
                RideClient rideClient = SelectRideFromList(_rideService.GetAllRides().ToList());

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
                _rideService.AddReview(reviewRequest);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                AddReview();
            }
        }

        #endregion

        #region GetDriverReview

        public static void GetDriverReviews(Guid rideId)
        {
            try
            {
                ICollection<ReviewClient> reviews = _rideService.GetDriverReviews(rideId);

                List<ReviewClient> reviewsList = new List<ReviewClient>(reviews);

                DisplayAllReviews(reviewsList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                PossibleActionsToBeDoneByLoggedUser();
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