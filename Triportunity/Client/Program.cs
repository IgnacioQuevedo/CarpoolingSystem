﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Client.Objects.ClientModels;
using Client.Objects.EnumsModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleImageModels;
using Client.Objects.VehicleModels;
using Client.Services;
using Common;

namespace Client
{
    internal class Program
    {
        private static User _userLogged;
        private static string _optionSelected;
        private static byte[] _messageInBytes;
        private static Socket _clientSocket;
        private static bool _closeApp;

        public static void Main(string[] args)
        {
            _clientSocket = NetworkHelper.ConnectWithServer();
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
                    PossibleActionsToBeDoneByUser();
                }
            }
        }

        #region Main Menu Options

        private static void MainMenuOptions()
        {
            Console.WriteLine("Welcome to Triportunity App");
            Console.WriteLine("Digit the number of your query");

            Console.WriteLine("1- Sign In");
            Console.WriteLine("2- Sign Up");
            Console.WriteLine("3- Who are we?");
            Console.WriteLine("4- Exit app");
        }

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
                    new RegisterUserRequest(usernameRegister, passwordRegister, repeatedPassword,
                        driverAspectsOfClient);

                UserService.RegisterClient(clientToRegister);
                Console.WriteLine("Want to login?");
                if (Console.ReadLine().Equals("Y"))
                {
                    var loginClient =
                        new LoginUserRequest(clientToRegister.Username, clientToRegister.Password);

                    _userLogged = UserService.LoginClient(loginClient);
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
                //_userLogged = UserService.LoginClient(loginRequest);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                LoginOption();
            }
        }


        #endregion

        #region Driver creation

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


        public static void PossibleActionsToBeDoneByUser()
        {
            if (_userLogged.DriverAspects is null)
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
                    var clientWithUpdates = new UpdateUserRequestModel
                        (_userLogged.Id, driverAspects);

                    _userLogged = UserService.UpdateClient(clientWithUpdates);
                    break;

                case "1":
                    CreateRide();
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

        #endregion


        private static void CreateRide()
        {
            Console.WriteLine("You will have to complete the following steps to have your ride created. Let's start with the creation of your ride!");

            Console.WriteLine("Select the initial location of your ride");

            int amountOfCities = CitiesEnum.GetValues(typeof(CitiesEnum)).Length;

            for (int i = 1; i <= amountOfCities; i++)
            {
                string cityName = Enum.GetName(typeof(CitiesEnum), i);
                Console.WriteLine($"Select {i}- {cityName}");
            }

        }


        #region General menu functions
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
        #endregion
    }
}