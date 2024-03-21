﻿using System;
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
                        Console.WriteLine("Insert a valid digit, please.");
                        Console.WriteLine("Returning to main menu...");
                        Thread.Sleep(1000);
                        Console.WriteLine("");
                        break;
                }
            }
        }

        private static bool CloseAppOption()
        {
            bool appFunctional = false;
            string closingMessage = "Closing";
            
            ShowMessageWithDelay(closingMessage, 300);
            Console.WriteLine("Closed App with success!");
            return appFunctional;
        }

        private static void ShowMessageWithDelay(string closingMessage, int delayTime)
        {
            Console.WriteLine(closingMessage);
            string dots = "";
            
            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(delayTime);
                dots +=  ".";
                closingMessage += ".";
                Console.WriteLine(closingMessage);
            }
        }

        private static void AboutUsOption()
        {
            Console.WriteLine("lorem ipsum dolor");
            Console.WriteLine("Enter any key to go back to the main menu");
            Console.ReadLine();
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
    }
}