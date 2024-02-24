using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace TG_Bot_MVC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Example how to use any class
            //var context = new LibraryContext();
            //var localAPI = new LocalAPI(context);
            //var configWorker = new ConfigWorker();

            //var users = localAPI.GetUsers();

            //foreach (var user in users)
            //{
            //    Console.WriteLine($"{user.UserName} - {user.IdUser} - {user.Status.StatusName}");
            //}

            //Console.WriteLine(configWorker.GetConnectionString());
            //Console.WriteLine(configWorker.GetLoggerString());
            //Console.WriteLine(configWorker.GetBotToken());

            //var Admins = configWorker.GetAdmins();
            //foreach (var admin in Admins)
            //{
            //    Console.WriteLine(admin);
            //}
            Parser.ParseHTML();
        }
    }
}
