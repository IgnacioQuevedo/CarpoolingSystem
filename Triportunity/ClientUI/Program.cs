using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Client.Objects.ClientModels;
using Client.Objects.VehicleModels;
using Client.Services;

namespace ClientUI
{
    internal class Program
    {
        static Client.Objects.ClientModels.Client _clientLogged = null;

        public static void Main(string[] args)
        {
            bool appFunctional = true;
            string optionSelected;

            while (appFunctional)
            {
                MainMenuOptions();

                 optionSelected = Console.ReadLine();

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

                if (_clientLogged != null)
                {
                    //Indicates that the user has been logged
                    //Here we need to display
                    if (_clientLogged.DriverAspects == null)
                    {
                        Console.WriteLine("Select 0 if you want to be registered as a driver too");
                        Console.WriteLine("Select 1- To create a Ride");
                        Console.WriteLine("Select 2- To join a Ride");
                        Console.WriteLine("Select 3- To view all your created rides");
                        Console.WriteLine("Select 4- To close the app");

                        optionSelected = Console.ReadLine();
                        
                        switch (optionSelected)
                        {
                            case "0":
                                DriverInfo driverAspects = CreateDriver();
                                UpdateClientRequestModel clientWithUpdates = new UpdateClientRequestModel
                                    (_clientLogged.Id,driverAspects);
                                
                                UserService.UpdateClient(clientWithUpdates);
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

        private static void gWrongDigitInserted()
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
            DriverInfo driverAspectsOfClient = null;
            
            try
            {
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
                    driverAspectsOfClient = CreateDriver();
                }

                ShowMessageWithDelay("Registering", 500);

                RegisterClientRequest clientToRegister =
                    new RegisterClientRequest(usernameRegister, passwordRegister, repeatedPassword,
                        driverAspectsOfClient);
                //ServiceMethod that will create the user.
                UserService.RegisterClient(clientToRegister);
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
            string ci;
            string addNewVehicle = "Y";
            ICollection<Vehicle> vehicles = new List<Vehicle>();

            Console.WriteLine("Insert your Ci for the registration");
            ci = Console.ReadLine();

            while (addNewVehicle.Equals("Y"))
            {
                Console.WriteLine("Insert a image of your Vehicle");
                //This must be fixed in a future.
                VehicleImage vehicleImage = null;
                Vehicle newVehicle = new Vehicle(vehicleImage);

                vehicles.Add(newVehicle);
                Console.WriteLine("Vehicle added, do you want to add a new vehicle?");
                Console.WriteLine("If yes - Enter 'Y'");
                Console.WriteLine("If not - Enter 'N'");

                addNewVehicle = Console.ReadLine();
            }

            DriverInfo driverAspectsOfClient = new DriverInfo(ci, vehicles);
            return driverAspectsOfClient;
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
    }
}