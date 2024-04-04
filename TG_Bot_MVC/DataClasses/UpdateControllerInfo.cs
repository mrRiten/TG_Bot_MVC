using Telegram.Bot.Types;

namespace TG_Bot_MVC.DataClasses
{
    internal class UpdateControllerInfo
    {
        public Update Update;
        public UserUpdate UserUpdate;
        public CancellationToken CancellationToken;
    }
}
