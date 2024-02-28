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

            //var user = localAPI.GetUser(1);

            //Console.WriteLine($"{user.UserName} - {user.IdUser} - {user.Status.StatusName}");

            //Console.WriteLine(configWorker.GetConnectionString());
            //Console.WriteLine(configWorker.GetLoggerString());
            //Console.WriteLine(configWorker.GetBotToken());

            // var Admins = configWorker.GetAdmins();
            //foreach (var admin in Admins)
            //{
            //    Console.WriteLine(admin);
            //}
            //localAPI.AddReplasementLesson(
            //    localAPI.TryGetGroupId(Parser.Group),
            //    localAPI.GetWeekOfScheduleId(Parser.WeekOfSchedule), 
            //    Parser.Json
            //    );
            Parser.MainParse();
            //var repl = localAPI.GetReplasementLesson(1);
            //Console.WriteLine($"{repl.WeekOfSchedule.WeekOfScheduleName} {repl.Group.GroupName} - {repl.SerializeDataLessons}");
        }
    }
}
