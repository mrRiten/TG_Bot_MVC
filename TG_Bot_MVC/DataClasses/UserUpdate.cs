using Telegram.Bot.Types;

namespace TG_Bot_MVC.DataClasses
{
    public class UserUpdate
    {
        public Message UserMessage { get; set; }
        public DateTime DateToRequest { get; set; }
    }
}
