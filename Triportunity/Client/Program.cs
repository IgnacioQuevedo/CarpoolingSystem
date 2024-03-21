using System;
using Client.Services;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string optionSelected = "";
            bool executeOptions = true;
            
            
            while (executeOptions)
            {
                MainMenuMsgs();
                optionSelected = Console.ReadLine();
                
                switch (optionSelected)
                {
                    case "1":
                        LoginMsgs();
                        break;

                    case "2":
                        Console.WriteLine("Your username will be:");
                        string usernameRegister = Console.ReadLine();
                        Console.WriteLine("Register your password:");
                        string passwordRegister = Console.ReadLine();
                        Console.WriteLine("Insert the same password as above:");
                        string repeatedPassword = Console.ReadLine();
                        //ServiceMethod that will create the user.
                        break;
                    
                    case "3":
                        Console.WriteLine("lorem ipsum dolor");
                        Console.WriteLine("Press 1 to go back to main menu");
                        break;
                    case "4":
                        Console.WriteLine("Closing");
                        executeOptions = false;
                        break;
                    default:
                        Console.WriteLine("Insert a valid digit, please.");
                        break;
                }
            }
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