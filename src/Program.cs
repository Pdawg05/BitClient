using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using BitcoinClient;
using net.vieapps.Components.Utility;
using Figgle;
using System.IO;

namespace ManualBitcoinTransaction
{
    class Program
    {
        private static bool WalletExists()
        {
            if (!File.Exists(WalletGeneration.WalletFilePath))
            {
                return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            WalletGeneration wallet = null;
            ConsoleCommands.IntroBanner();


            if (!Directory.Exists(WalletGeneration.WalletDirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(WalletGeneration.WalletDirectoryPath);
                }

                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Failed to create neccessary program folders. Please run as administrator!");
                }
            }

            if (!WalletExists())
            {
                string response = String.Empty;

                while (response != "y")
                {
                    Console.WriteLine("No existing wallet has been found, would you like to make one? [Y/N]");
                    Console.Write(" >>>");
                    response = Console.ReadLine().ToLower();

                    if (response == "y")
                    {
                        wallet = new WalletGeneration(true);
                        Console.WriteLine("Wallet created at " + WalletGeneration.WalletFilePath);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Your new bitcoin address is: " + wallet.Address);
                        Console.ResetColor();
                        break;
                    }

                    else if (response == "n")
                    {
                        Console.WriteLine("Cannot run program without a wallet, exiting...");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine("Invalid response... Try again!");
                    }
                }

            }

            else
            {
                // Load in wallet
                wallet = WalletGeneration.LoadKeysOnDisk();
            }

            while (true)
            {
                Console.Write(" >>> ");
                string command = Console.ReadLine().ToLower();
                
                switch (command)
                {
                    case "help":
                        ConsoleCommands.HelpCommand();
                        break;
                    case "wallet details":
                        ConsoleCommands.GetWalletDetails(wallet);
                        break;
                    case "delete wallet":
                        ConsoleCommands.DeleteWallet();
                        break; 
                    case "clear":
                        Console.Clear();
                        break;
                    case "quit":
                        Environment.Exit(0);
                        break;
                }
            }
        }


    }
}
