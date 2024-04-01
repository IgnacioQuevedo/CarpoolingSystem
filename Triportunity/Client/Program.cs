﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Client.Objects.ClientModels;
using Client.Objects.VehicleModels;
using Client.Services;

namespace Client
{
    internal class Program
    {
        readonly UserService _userService = new UserService();

        public static void Main(string[] args)
        {
            bool appFunctional = true;
            bool userLogged = false;

            while (appFunctional)
            {
                MainMenuOptions();

                var optionSelected = Console.ReadLine();

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
                        appFunctional = CloseAppOption();
                        break;
                    default:
                        WrongDigitInserted();
                        break;
                }

                if (userLogged)
                {
                    //All the new functionalities to be done.
                }
            }
        }

        #region Main Menu Options

        private static void WrongDigitInserted()
        {
            Console.WriteLine("Insert a valid digit, please.");
            string goBackMessage = "Returning to main menu";
            ShowMessageWithDelay(goBackMessage, 1000);
            Console.WriteLine("");
        }

        private static bool CloseAppOption()
        {
            bool appFunctional = false;
            string closingMessage = "Closing";

            ShowMessageWithDelay(closingMessage, 300);
            Console.WriteLine("");
            Console.WriteLine("Closed App with success!");
            return appFunctional;
        }

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
                Console.WriteLine("Triportinuty is a travel web app");
            }

            Console.WriteLine("");
            Console.WriteLine("Enter any key to go back to the main menu");
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();
            ShowMessageWithDelay("Going back to Main Menu", 500);
            Console.WriteLine();
        }

        private static void RegisterOption()
        {
            ICollection<Vehicle> vehicles = new List<Vehicle>();
            string addAnewVehicle = "Y";
            string ci = "";

            Console.WriteLine("Your username will be:");
            string usernameRegister = Console.ReadLine();
            Console.WriteLine("Register your password:");
            string passwordRegister = Console.ReadLine();
            Console.WriteLine("Insert the same password as above:");
            string repeatedPassword = Console.ReadLine();
            Console.WriteLine("Do you want to be register as a driver?");
            Console.WriteLine("Insert 'Y' for Yes or 'N' for No");

            if (Console.ReadLine().Equals('Y'))
            {
                Console.WriteLine("Insert your Ci for the registration");
                ci = Console.ReadLine();
                
                while (addAnewVehicle.Equals("Y"))
                {
                    Console.WriteLine("Insert a image of your Vehicle");
                    //This must be fixed in a future.
                    VehicleImage vehicleImage = null;
                    Vehicle newVehicle = new Vehicle(vehicleImage);
                    vehicles.Add(newVehicle);
                    
                    Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                    Console.WriteLine("If yes - Enter 'Y'");
                    Console.WriteLine("If not - Enter 'N'");
                    addAnewVehicle = Console.ReadLine();
                }
            }

            ShowMessageWithDelay("Registering", 500);
            RegisterClientRequest clientToRegister =
                new RegisterClientRequest(ci,usernameRegister, passwordRegister, repeatedPassword, vehicles);
            
            //ServiceMethod that will create the user.
            UserService.RegisterClient(clientToRegister);
        }

        private static void LoginOption()
        {
            try
            {
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();
            
                //ServiceMethod that will login the user into the app
                LoginClientRequest loginRequest = new LoginClientRequest(username, password);
                UserService.LoginClient(loginRequest);
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
    }
}