using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using QUT4Net.Models;

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

            while (true)
            {
                Console.Write("Student ID Number: ");
                var idNum = Console.ReadLine();

                var credits = await client.Profile.GetStudentPrintCredits(idNum);

                if (credits == -1)
                {
                    Console.WriteLine("This student doesn't seem to exist.");
                }
                else
                {
                    Console.WriteLine($"{idNum}'s Print Credits: ${credits}");
                }
                Console.WriteLine();
            }

            while (true)
            {
                Console.WriteLine($"Student Title: {await client.Profile.GetStudentTitle()}");
                Console.WriteLine($"Student Type: {await client.Profile.GetStudentType()}");
                Console.WriteLine($"Personal Email: {await client.Profile.GetPersonalEmail()}");
                Console.WriteLine($"QUT Email: {await client.Profile.GetQUTEmail()}");
                Console.WriteLine($"Phone Number: {await client.Profile.GetPersonalPhoneNumber()}");
                Console.WriteLine($"Address: {await client.Profile.GetHomeAddress()}");
                Console.WriteLine($"Print Credits: ${await client.Profile.GetPrintCredits()}");
                Console.WriteLine($"Current GPA: {await client.Study.GetCurrentGPA()}");
                Console.ReadLine();
            }
        }
    }
}