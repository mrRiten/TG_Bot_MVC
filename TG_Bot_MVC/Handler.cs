using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    public abstract class IUpdateHandler
    {
        public IUpdateHandler(LocalAPI localAPI)
        {
            this.localAPI = localAPI;
        }

        public readonly LocalAPI localAPI;
        public readonly IUpdateHandler? nextHandler;

        public abstract bool Active(Message userMessage);

        public bool? CallNextHandler(Message userMessage)
        {
            return Active(userMessage);
        }
    }

    public class MainHandler : IUpdateHandler
    {
        public MainHandler(IUpdateHandler? nextHandler, LocalAPI localAPI) : base(localAPI)
        {
            this.nextHandler = nextHandler;
        }

        public new readonly IUpdateHandler? nextHandler;

        public override bool Active(Message userMessage)
        {
            var user = localAPI.TryGetUser(userMessage.Chat.Id, userMessage.Chat.FirstName);

            if (user.StatusId == 1)
            {
                return nextHandler?.CallNextHandler(userMessage) ?? true;
            }
            return false;
        }
    }

    public class TestHandler : IUpdateHandler
    {
        public TestHandler(IUpdateHandler? nextHandler, LocalAPI localAPI) : base(localAPI)
        {
            this.nextHandler = nextHandler;
        }

        public new readonly IUpdateHandler? nextHandler;

        public override bool Active(Message userMessage)
        {
            var user = localAPI.GetFullInfoUser(userMessage.Chat.Id);

            if (user.Setting.Group.GroupName == "ИС1-21")
            {
                return nextHandler?.CallNextHandler(userMessage) ?? true;
            }
            return false;
        }
    }

}
