using Figgle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BitcoinClient
{
    public static class ConsoleCommands
    {
        public static void GetWalletDetails (WalletGeneration wallet)
        {
            Console.WriteLine("Here are your wallet details: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("BTC Address: " + wallet.Address);
            Console.WriteLine("Public Key: " + wallet.PublicKeyStr);
            Console.ResetColor();

        }

        public static void DeleteWallet()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Are you sure you want to delete your wallet? You will lose ALL your bitcoins! Please type CONFIRM to proceed.");
            Console.ResetColor();
            string confirmation = Console.ReadLine();
            if (confirmation != "CONFIRM")
            {
                return;
            }

            else
            {
                File.Delete(WalletGeneration.WalletFilePath);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Your wallet has been deleted! Please restart the program.");
                Console.ResetColor();
                Environment.Exit(0);
            }
        }
        public static void HelpCommand()
        {
            //IDictionary<string, string> commands = new Dictionary<string, string>();

            Console.WriteLine("Available commands: ");
            Console.WriteLine("clear");
            Console.WriteLine("quit");
            Console.WriteLine("create wallet");
            Console.WriteLine("wallet details");
            Console.WriteLine("check available btc");
            Console.WriteLine("send btc");
            Console.WriteLine("delete wallet");
        }
        public static void IntroBanner()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(FiggleFonts.Standard.Render("BitClient"));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" - Created by Pdawg05 | github.com/Pdawg05");
            Console.ResetColor();
            Console.WriteLine("\n\n");
        }
    }
}
