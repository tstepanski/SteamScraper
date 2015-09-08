using System;
using System.Linq;
using SteamPageParser;

namespace SteamPageParserRunner
{
    public class Program
    {
        public static void Main()
        {
            Navigator.GetSteamApps().ForAll(ShowApp);
            
            Console.In.ReadLine();
        }

        private static void ShowApp(SteamApp app)
        {
            Console.Out.WriteLine(app.Title);

            foreach (var package in app.Packages)
            {
                Console.Out.WriteLine("Package:");
                Console.Out.WriteLine($"\t{package.Title}");
                Console.Out.WriteLine($"\t${package.CurrentPrice}");
                Console.Out.WriteLine("------------");
            }

            Console.Out.WriteLine("-------------------------------------------------------------");
        }
    }
}