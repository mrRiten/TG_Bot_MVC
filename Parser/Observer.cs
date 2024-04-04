using TG_Bot_MVC;

namespace Parser
{
    internal class ParserObserverCreator : IObserver
    {
        public void Update()
        {
            var builder = new ScheduleBuilder();
            builder.MainBuild();
        }
    }

    internal class ParserObserverDeletor : IObserver
    {
        public void Update()
        {
            var _localAPI = new LocalAPI(new LibraryContext());
            int targetDataToDel = (int)DateTime.Today.AddDays(-2).DayOfWeek;
            _localAPI.DelCorrectSchedules(targetDataToDel);
        }
    }
}
