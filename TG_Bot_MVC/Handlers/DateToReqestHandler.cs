using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Handlers
{
    public class DateToRequestHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            if (userUpdate.UserMessage.Text == "⬅️ Предыдущее")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(-1);
            }
            else if (userUpdate.UserMessage.Text == "Следующее ➡️")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(1);
            }
            return nextHandler?.CallNextHandler(userUpdate) ?? true;
        }
    }
}
