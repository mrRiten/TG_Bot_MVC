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
            Logger.LogInfo("Call ParserObserverDelitor");
        }
    }
}
