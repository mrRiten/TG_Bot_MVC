namespace TG_Bot_MVC.DataClasses
{
    public class DDoSData
    {
        public Dictionary<long, DateTime> dictUserDate = new();
        public Dictionary<long, int> dictUserWarning = new();
        public TimeSpan timeLimit = new TimeSpan(0, 0, 1);
    }
}
