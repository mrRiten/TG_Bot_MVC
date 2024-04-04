using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TG_Bot_MVC.DataClasses;
using TG_Bot_MVC.Keyboards;

namespace TG_Bot_MVC.Controller
{
    internal class CommandController(BotView botView, LocalAPI localAPI, DDoSData dDoSData) : BaseController(botView, localAPI, dDoSData)
    {
        public override async Task ActiveController(Update update, UserUpdate userUpdate, CancellationToken cancellationToken)
        {
            var message = update.Message;
            var inlineBoards = new Inline();
            await _view.SendCommandResponse(
                message.Chat.Id,
                message.Text,
                inlineBoards.inlineKeyboardSetting,
                cancellationToken);
        }
    }
}
