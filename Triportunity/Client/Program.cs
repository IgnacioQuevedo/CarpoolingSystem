using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Client.Objects.ClientModels;
using Client.Objects.VehicleModels;
using Client.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

namespace ClientUI
{
    internal class Program
    {
        private static byte[] _messageInBytes;
        private static readonly bool _userLogged = false;
        private static bool _closeApp;
        private static Socket _clientSocket;

        public static void Main(string[] args)
        {
            _clientSocket = NetworkHelper.ConnectWithServer();
            while (!_closeApp)
            {
                if (_clientLogged is null)
                {
                    MainMenuOptions();

                    _optionSelected = Console.ReadLine();
    switch (optionSelected)
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

                if (_userLogged)
                {
                    {
                        if (_clientLogged.DriverAspects is null)
                        {
                            Console.WriteLine("Select 0- If you want to be registered as a driver");
                        }

                        Console.WriteLine("Select 1- To create a Ride");
                        Console.WriteLine("Select 2- To join a Ride");
                        Console.WriteLine("Select 3- To view all your created rides");
                        Console.WriteLine("Select 4- To close the app");
                        _optionSelected = Console.ReadLine();

                        switch (_optionSelected)
                        {
                            case "0":
                                var driverAspects = CreateDriver();
                                var clientWithUpdates = new UpdateClientRequestModel
                                    (_clientLogged.Id, driverAspects);

                                _clientLogged = UserService.UpdateClient(clientWithUpdates);
                                break;

                            case "1":
                                //  ride method
                                break;

                            case "2":
                                // Join ride
                                break;
                            case "3":
                                // View all the rides of the client logged and filtered by a criteria (such as Destination,Price,AllowanceOfPets,etc)
                                // And so on let the client update their published rides and furthermore delete them
                                break;
                            case "4":
                                // Close the app
                                CloseAppOption();
                                break;

                            default:
                                WrongDigitInserted();
                                break;
                        }
                    }
                }
            }
        }

        #region Main Menu Options

        private static void WrongDigitInserted()
        {
            Console.WriteLine("Insert a valid digit, please.");
            ShowMessageWithDelay("Returning to main menu", 1000);
            Console.WriteLine("");
        }

        private static void CloseAppOption()
        {
            ShowMessageWithDelay("Closing", 300);
            Console.WriteLine("");
            Console.WriteLine("Closed App with success!");
            NetworkHelper.CloseSocketConnections(_clientSocket);
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
            ShowMessageWithDelay("Going back to Main Menu", 500);
            Console.WriteLine();
        }

        private static void RegisterOption()
        {
            DriverInfo driverAspectsOfClient = null;

            try
            {
                Console.WriteLine("Your username will be:");
                var usernameRegister = Console.ReadLine();
                Console.WriteLine("Register your password:");
                var passwordRegister = Console.ReadLine();
                Console.WriteLine("Insert the same password as above:");
                var repeatedPassword = Console.ReadLine();

                Console.WriteLine("Do you want to be register as a driver?");
                Console.WriteLine("Insert 'Y' for Yes or 'N' for No");
                if (Console.ReadLine().Equals("Y"))
                {
                    driverAspectsOfClient = CreateDriver();
                }

                ShowMessageWithDelay("Registering", 500);

                var clientToRegister =
                    new RegisterClientRequest(usernameRegister, passwordRegister, repeatedPassword,
                        driverAspectsOfClient);

                UserService.RegisterClient(clientToRegister);
                Console.WriteLine("Want to login?");
                if (Console.ReadLine().Equals("Y"))
                {
                    var loginClient =
                        new LoginClientRequest(clientToRegister.Username, clientToRegister.Password);
                    
                    _clientLogged = UserService.LoginClient(loginClient);
                }
              string registerInfo = usernameRegister + ";" + passwordRegister + ";" + repeatedPassword;
            _messageInBytes = NetworkHelper.EncodeMsgIntoBytes(registerInfo);
              //Need to pass DriverInfo too
            _clientSocket.Send(_messageInBytes);
            //ServiceMethod that will create the user (DO AS A REFACTOR IN A TIME)
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("Redo all again but without the above error.");
                RegisterOption();
            }
        }

        private static DriverInfo CreateDriver()
        {
            string ci = "";
            string addNewVehicle = "Y";
            ICollection<Vehicle> vehicles = new List<Vehicle>();

            Console.WriteLine("Insert your Ci for the registration");
            ci = Console.ReadLine();

            while (addNewVehicle.Equals("Y"))
            {
                Console.WriteLine("Insert a image of your Vehicle");
                //This must be fixed in a future.
                VehicleImage vehicleImage = null;
                var newVehicle = new Vehicle(vehicleImage);

                vehicles.Add(newVehicle);
                Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                Console.WriteLine("If yes - Enter 'Y'");
                Console.WriteLine("If not - Enter 'N'");

                addNewVehicle = Console.ReadLine();
            }

            var driverAspectsOfClient = new DriverInfo(ci, vehicles);
            return driverAspectsOfClient;
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
              
                string loginInfo = username + ";" + password;
                _messageInBytes = NetworkHelper.EncodeMsgIntoBytes(loginInfo);
              
                _clientSocket.Send(_messageInBytes);
            //ServiceMethod that will login the user into the app (DO AS A REFACTOR IN A TIME)
                
                _clientLogged = UserService.LoginClient(loginRequest);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                LoginOption();
            }
        }

        private static void MainMenuOptions()
        {
            Console.WriteLine("Welcome to Triportunity App");
            Console.WriteLine("Digit the number of your query");

            Console.WriteLine("1- Sign In");
            Console.WriteLine("2- Sign Up");
            Console.WriteLine("3- Who are we?");
            Console.WriteLine("4- Exit app");
        }

        #endregion

        private static void ShowMessageWithDelay(string closingMessage, int delayTime)
        {
            Console.Write(closingMessage);
            string dots = "";

            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(delayTime);
                dots += ".";
                Console.Write(dots);
            }

            Console.WriteLine("");
        }
    }
}