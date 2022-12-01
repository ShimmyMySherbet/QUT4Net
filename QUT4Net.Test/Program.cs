using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QUT4Net
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = Console.ReadLine();

            Console.Clear();

            var client = new HiQClient();

            if (!await client.LoginAsync(username, password))
            {
                Console.WriteLine("Login failed");
                return;
            }

            Console.WriteLine("Logged in.");

            Console.WriteLine("Started scraping unit info... (90 cap)");

            var sw = new Stopwatch();
            sw.Start();
            var units = await client.Collection.CollectUnitInfoAsync();
            sw.Stop();

            var txt = JsonConvert.SerializeObject(units);
            File.WriteAllText("units.json", txt);

            Console.WriteLine($"Completed in {sw.ElapsedMilliseconds / 1000} sec");
        }
    }
}