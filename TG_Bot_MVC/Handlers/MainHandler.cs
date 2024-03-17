using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Handlers
{
    public class MainHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            var user = localAPI.TryGetUser(userUpdate.UserMessage.Chat.Id, userUpdate.UserMessage.Chat.FirstName);

            if (user.IsBanned == false)
            {
                return nextHandler?.CallNextHandler(userUpdate) ?? true;
            }
            return false;
        }
    }
}
