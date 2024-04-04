using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Telegram.Bot.Types;
using TG_Bot_MVC.DataClasses;
using TG_Bot_MVC.Keyboards;

namespace TG_Bot_MVC.Controller
{
    internal class CallbackController(BotView botView, LocalAPI localAPI, DDoSData dDoSData) : BaseController(botView, localAPI, dDoSData)
    {

        public override async Task ActiveController(Update update, UserUpdate userUpdate, CancellationToken cancellationToken)
        {
            var callback = update.CallbackQuery;

            if (callback.Data.StartsWith("S"))
            {
                await StatusActive(callback, userUpdate);
            }
        }

        private async Task StatusActive(CallbackQuery callback, UserUpdate userUpdate)
        {
            if (callback.Data == "SStudent")
            {
                _localAPI.SetStatus(userUpdate.UserMessage.Chat.Id, 1);
                await _view.EditInlineMessage(
                    userUpdate.UserMessage.Chat.Id,
                    userUpdate.UserMessage.MessageId,
                    "Ваш статус изменён на Student");
                
            }
            else if (callback.Data == "STeacher")
            {
                _localAPI.SetStatus(userUpdate.UserMessage.Chat.Id, 2);
                await _view.EditInlineMessage(
                    userUpdate.UserMessage.Chat.Id,
                    userUpdate.UserMessage.MessageId,
                    "Ваш статус изменён на Teacher");
            }
        }
    }
}
