using System;
using System.Threading;
using Client.Services;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string optionSelected = "";
            bool appFunctional = true;
            
            
            while (appFunctional)
            {
                MainMenuMsgs();
                optionSelected = Console.ReadLine();
                
                switch (optionSelected)
                {
                    case "1":
                        LoginMsgs();
                        break;

                    case "2":
                        RegisterMsgs();
                        break;
                    
                    case "3":
                        AboutUsMsgs();
                        break;
                    case "4":
                        appFunctional = CloseApp();
                        break;
                    default:
                        Console.WriteLine("Insert a valid digit, please.");
                        break;
                }
            }
        }

        private static bool CloseApp()
        {
            bool closeApp;
            Console.WriteLine("Closing");
            for (int i = 0; i < 8; i++)
            {
                Thread.Sleep(300);
                Console.WriteLine(".");
            }
            return false;
        }

        private static void AboutUsMsgs()
        {
            Console.WriteLine("lorem ipsum dolor");
            Console.WriteLine("Press 1 to go back to main menu");
        }

        private static void RegisterMsgs()
        {
            Console.WriteLine("Your username will be:");
            string usernameRegister = Console.ReadLine();
            Console.WriteLine("Register your password:");
            string passwordRegister = Console.ReadLine();
            Console.WriteLine("Insert the same password as above:");
            string repeatedPassword = Console.ReadLine();
            //ServiceMethod that will create the user.
        }

        private static void LoginMsgs()
        {
            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            Console.WriteLine("Password:");
            string password = Console.ReadLine();
            //ServiceMethod that will login the user into the app
        }

        private static void MainMenuMsgs()
        {
            Console.WriteLine("Welcome to Triportunity App");
            Console.WriteLine("Digit the number of your query");
            
            Console.WriteLine("1- Sign In");
            Console.WriteLine("2- Sign Up");
            Console.WriteLine("3- Who are we?");
            Console.WriteLine("4- Exit app");
        }
    }
}