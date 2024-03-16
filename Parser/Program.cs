using TG_Bot_MVC;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new ParserHTML();
            parser.MainParse("https://menu.sttec.yar.ru/timetable/rasp_first.html");
            parser.MainParse("https://menu.sttec.yar.ru/timetable/rasp_second.html");

            var builder = new ScheduleBuilder();
            builder.MainBuild(parser.document);
            //var api = new LocalAPI(new LibraryContext());
            //api.DelReplasementLessons(DateTime.Today);
            //api.DelCorrectSchedules(DateTime.Today);
        }
    }
}