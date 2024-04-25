using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
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

        //private static byte[] _messageInBytes;
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
            // _rideService = new RideService(clientSocket);

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
                    //This is when the user is logged in
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
            Console.WriteLine("Insert a valid digit, please.");
            Console.WriteLine("");
        }

        private static void CloseAppOption()
        {
            Console.WriteLine("");
            Console.WriteLine("Closed App with success!");
            NetworkHelper.CloseSocketConnections(clientSocket);
            _closeApp = true;
        }

        private static void AboutUsOption()
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

        private static void RegisterOption()
        {
            DriverInfoClient driverAspectsOfClient = null;

            try
            {
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

                UserService.RegisterClient(clientSocket, clientToRegister);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Redo all again but without this error: " + exception.Message);
                RegisterOption();
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
                _userLogged = UserService.LoginClient(clientSocket, loginUserRequest);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                LoginOption();
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
                Console.WriteLine("Select 5 - To close the app");

                _optionSelected = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Select 1 - If you want to be registered as a driver");
                Console.WriteLine("Select 2 - To join a Ride");
                Console.WriteLine("Select 3 - To Quit a Ride");
                Console.WriteLine("Select 4 - To close the app");
                _optionSelected = Console.ReadLine();
            }

            if (!int.TryParse(_optionSelected, out int optionParsed) || optionParsed < 1 || optionParsed > 5)
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
                        ModifyRide();
                    else
                        CloseAppOption();
                    break;

                case 5:
                    if (_userLogged.DriverAspects != null)
                        CloseAppOption();
                    break;

                default:
                    WrongDigitInserted();
                    break;
            }
        }

        #endregion


        #region Driver creation

        private static void CreateDriver(Guid userRegisteredId)
        {
            string addNewVehicle = "Y";

            while (addNewVehicle.Equals("Y"))
            {
                Console.WriteLine("Please enter the model of the vehicle");
                string carModel = Console.ReadLine();
                Console.WriteLine("Please enter the path of the vehicle image");
                string path = Console.ReadLine();
                
                UserService.CreateDriver(clientSocket, userRegisteredId);
                UserService.AddVehicle(clientSocket, userRegisteredId,carModel,path);
                Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                Console.WriteLine("If yes - Enter 'Y'");
                Console.WriteLine("If not - Enter 'N'");
                addNewVehicle = Console.ReadLine();
            }
        }

        #endregion


        #region Create Ride

        private static void CreateRide()
        {
            Console.WriteLine(
                "You will have to complete the following steps to have your ride created. Let's start with the creation of your ride!");

            List<UserClient> passengers = new List<UserClient>();

            Guid vehicleIdSelected = Guid.Parse(PickVehicle());

            string locationMode = "initial";
            CitiesEnum initialLocation = PickLocation(locationMode);

            locationMode = "ending";
            CitiesEnum endingLocation = PickLocation(locationMode);

            DateTime departureDate = PickDepartureDate();

            int availableSeats = PickAmountOfAvailableSeats();

            double pricePerPerson = IntroducePricePerPerson();

            bool petsAllowed = DecideIfPetsAreAllowed();

            string photoPath = IntroducePhotoPath();

            CreateRideRequest rideReq = new CreateRideRequest(_userLogged, vehicleIdSelected, passengers,
                initialLocation, endingLocation,
                departureDate, availableSeats, pricePerPerson, petsAllowed, photoPath);

            _rideService.CreateRide(rideReq);
        }


        private static string IntroducePhotoPath()
        {
            Console.Write("Introduce your photo path");
            return Console.ReadLine();
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

            ICollection<VehicleClient> vehicles = UserService.GetVehiclesByUserId(_userLogged.Id);

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
            List<RideClient> rides = (List<RideClient>)_rideService.GetAllRides();

            RideClient selectedRide = SelectRideFromList(rides);


            JoinRideRequest joinReq = new JoinRideRequest(selectedRide.Id, _userLogged);

            _rideService.JoinRide(joinReq);
        }

        private static RideClient SelectRideFromList(List<RideClient> rides)
        {
            Console.WriteLine("Select the ride you are aiming into");
            Console.WriteLine();

            DisplayAllRides(rides);

            _optionSelected = Console.ReadLine();

            if (int.TryParse(_optionSelected, out int optionValue))
            {
                if (optionValue <= rides.Count)
                {
                    RideClient rideSelected = rides[optionValue - 1];
                    Console.WriteLine(
                        $"You have selected the ride From: {rideSelected.InitialLocation} To: {rideSelected.EndingLocation}");
                    Console.WriteLine($"Departure time on: {rideSelected.DepartureTime.ToShortDateString()}");
                    Console.WriteLine("Price: ${rideSelected.PricePerPerson}");

                    Console.WriteLine("Do you want to see the details of the ride? -- 'Y' for Yes or 'N' for No");
                    string seeDetails = Console.ReadLine();
                    if (seeDetails == "Y")
                    {
                        Console.WriteLine("And at the moment are available " + rideSelected.AvailableSeats);
                        Console.WriteLine("Pets allowed: " + "rideSelected.PetsAllowed");

                        Console.WriteLine("Do you want to see the car image?");
                        if (seeDetails == "Y")
                        {
                            _rideService.GetCarImageById(rideSelected.Id);
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

        private static void DisplayAllRides(List<RideClient> rides)
        {
            int amountOfRides = rides.Count;

            RideClient actualRide = new RideClient();

            for (int i = 0; i < amountOfRides; i++)
            {
                actualRide = rides[i];
                Console.WriteLine(
                    $"1- From: {actualRide.InitialLocation} To: {actualRide.EndingLocation} Date of departure: {actualRide.DepartureTime.ToShortDateString()} Price per person: ${actualRide.PricePerPerson}");
            }
        }

        #endregion

        #region Modify Ride

        private static void ModifyRide()
        {
            ICollection<RideClient> ridesCollection = _rideService.GetAllRides();
            List<RideClient> ridesList = new List<RideClient>(ridesCollection);

            DisplayAllRides(ridesList);

            RideClient rideSelected = SelectRideFromList(ridesList);

            Console.WriteLine("You will have to complete the following steps to have your ride edited");

            string locationMode = "initial";
            CitiesEnum initialLocation = PickLocation(locationMode);

            locationMode = "ending";
            CitiesEnum endingLocation = PickLocation(locationMode);

            DateTime departureDate = PickDepartureDate();

            int availableSeats = PickAmountOfAvailableSeats();

            double pricePerPerson = IntroducePricePerPerson();

            bool petsAllowed = DecideIfPetsAreAllowed();

            string photoPath = IntroducePhotoPath();

            ModifyRideRequest modifyRideReq = new ModifyRideRequest(initialLocation, endingLocation, departureDate,
                pricePerPerson, petsAllowed, photoPath);

            _rideService.ModifyRide(modifyRideReq);
        }

        #endregion

        #region Quit Ride

        private static void QuitRide()
        {
            ICollection<RideClient> ridesCollection = _rideService.GetAllRides();
            List<RideClient> ridesList = new List<RideClient>(ridesCollection);

            DisplayAllRides(ridesList);

            RideClient rideSelected = SelectRideFromList(ridesList);

            QuitRideRequest quitRideReq = new QuitRideRequest(rideSelected.Id, _userLogged);

            _rideService.QuitRide(quitRideReq);
        }

        #endregion

        #region Delete Ride

        private static void DeleteRide()
        {
            ICollection<RideClient> ridesCollection = _rideService.GetAllRides();
            List<RideClient> ridesList = new List<RideClient>(ridesCollection);

            DisplayAllRides(ridesList);

            RideClient rideSelected = SelectRideFromList(ridesList);

            _rideService.DeleteRide(rideSelected.Id);
        }

        #endregion

        #region Disable Ride

        private static void DisableRide()
        {
            ICollection<RideClient> ridesCollection = _rideService.GetAllRides();
            List<RideClient> ridesList = new List<RideClient>(ridesCollection);

            DisplayAllRides(ridesList);

            RideClient rideSelected = SelectRideFromList(ridesList);

            _rideService.DisableRide(rideSelected.Id);
        }

        #endregion

        #region Get Ride Info

        private static void GetRideInfo()
        {
            ICollection<RideClient> ridesCollection = _rideService.GetAllRides();
            List<RideClient> ridesList = new List<RideClient>(ridesCollection);

            DisplayAllRides(ridesList);

            RideClient rideSelected = SelectRideFromList(ridesList);

            _rideService.GetRideById(rideSelected.Id);
        }

        #endregion

        #region Get Rides By Price

        private static void GetRidesByPrice()
        {
            Console.WriteLine("Introduce the minumun and maximum price you want to filter the rides by");

            Console.WriteLine("Introduce the minimum price");
            double minPrice = double.Parse(Console.ReadLine());

            Console.WriteLine("Introduce the maximum price");
            double maxPrice = double.Parse(Console.ReadLine());

            _rideService.GetRidesFilteredByPrice(minPrice, maxPrice);
        }

        #endregion

        #region Get Rides By Initial Location

        private static void GetRidesByInitialLocation()
        {
            Console.WriteLine("Introduce the initial location you want to filter the rides by");

            ShowCities();

            _optionSelected = Console.ReadLine();

            CitiesEnum initialLocation = PossibleCasesWhenPickingLocation(_optionSelected, "initial");

            _rideService.GetRidesFilteredByInitialLocation(initialLocation);
        }

        #endregion

        #region Get Rides By Ending Location

        private static void GetRidesByEndingLocation()
        {
            Console.WriteLine("Introduce the ending location you want to filter the rides by");

            ShowCities();

            _optionSelected = Console.ReadLine();

            CitiesEnum endingLocation = PossibleCasesWhenPickingLocation(_optionSelected, "ending");

            _rideService.GetRidesFilteredByEndingLocation(endingLocation);
        }

        #endregion
    }
}