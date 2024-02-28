**Examples how to use LocalAPI and ConfigWorker**

```C#
namespace TG_Bot_MVC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Example how to use any class
            var context = new LibraryContext();
            var localAPI = new LocalAPI(context);
            var configWorker = new ConfigWorker();

            var user = localAPI.GetUser(1);

            Console.WriteLine($"{user.UserName} - {user.IdUser} - {user.Status.StatusName}");

            Console.WriteLine(configWorker.GetConnectionString());
            Console.WriteLine(configWorker.GetLoggerString());
            Console.WriteLine(configWorker.GetBotToken());

            var Admins = configWorker.GetAdmins();
            foreach (var admin in Admins)
            {
                Console.WriteLine(admin);
            }

            localAPI.AddReplasementLesson(
                localAPI.TryGetGroupId("ИС1-21"),
                localAPI.GetWeekOfScheduleId("Знаменатель"),
                "{ {} {} {} {} {} {} {} {} }",
                DateTime.Today.AddDays(1)
                );

            var repl = localAPI.GetReplasementLesson(1, (int)DayOfWeek.Wednesday);
            Console.WriteLine($"{repl.WeekOfSchedule.WeekOfScheduleName} {repl.Group.GroupName} - {repl.SerializeDataLessons}");

            localAPI.AddUserSetting(2, 1);

            var userFull = localAPI.GetFullInfoUser(2);
            Console.WriteLine($"{userFull.IdUser} {userFull.UserName} {userFull.Status.StatusName} {userFull.Setting.IdSetting} {userFull.Setting.Group.Department.DepartmentName}");

            localAPI.SetMailingSetting(2, false);
            localAPI.SetTimeOfLessonsSetting(2, false);
            localAPI.SetGroupSetting(2, 2);

            userFull = localAPI.GetFullInfoUser(2);
            Console.WriteLine($"{userFull.IdUser} {userFull.UserName} {userFull.Status.StatusName} {userFull.Setting.IdSetting} {userFull.Setting.Group.Department.DepartmentName}");

            localAPI.DelReplasementLessons(DateTime.Today);
        }
    }
}
```
