using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    public abstract class BaseUpdateHandler(LocalAPI localAPI)
    {
        public readonly LocalAPI localAPI = localAPI;
        public readonly BaseUpdateHandler? nextHandler;

        public abstract bool Active(UserUpdate userUpdate);

        public bool? CallNextHandler(UserUpdate userUpdate)
        {
            return Active(userUpdate);
        }
    }

    public class MainHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            var user = localAPI.TryGetUser(userUpdate.UserMessage.Chat.Id, userUpdate.UserMessage.Chat.FirstName);

            if (user.StatusId == 1)
            {
                return nextHandler?.CallNextHandler(userUpdate) ?? true;
            }
            return false;
        }
    }

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


    public class DateHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            if (userUpdate.DateToRequest.DayOfWeek == DayOfWeek.Sunday && userUpdate.UserMessage.Text == "Следующее ➡️")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(1);
            }
            else if (userUpdate.DateToRequest.DayOfWeek == DayOfWeek.Sunday && userUpdate.UserMessage.Text == "Следующее ➡️")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(-1);
            }
            return nextHandler?.CallNextHandler(userUpdate) ?? true;
        }
    }

}
