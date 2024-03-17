using TG_Bot_MVC;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                MainUpdate(["https://menu.sttec.yar.ru/timetable/rasp_first.html", "https://menu.sttec.yar.ru/timetable/rasp_second.html"]);

                Thread.Sleep(TimeSpan.FromMinutes(20));
            }
        }

        static void MainUpdate(string[] pathToParse)
        {
            var observer = new ParserObserverCreator();
            var parser = new ParserHTML(observer);
            //var builder = new ScheduleBuilder();
            parser.MainParse(pathToParse);
            //builder.MainBuild();

            //var api = new LocalAPI(new LibraryContext());
            //api.DelReplasementLessons(6);
            //api.DelCorrectSchedules(6);
        }
    }
}