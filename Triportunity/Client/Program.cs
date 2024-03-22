using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Client.Services;

namespace Client
{
    internal class Program
    {
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
            Console.WriteLine("Your username will be:");
            string usernameRegister = Console.ReadLine();
            Console.WriteLine("Register your password:");
            string passwordRegister = Console.ReadLine();
            Console.WriteLine("Insert the same password as above:");
            string repeatedPassword = Console.ReadLine();
            //ServiceMethod that will create the user.
        }

        private static void LoginOption()
        {
            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            Console.WriteLine("Password:");
            string password = Console.ReadLine();
            //ServiceMethod that will login the user into the app
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