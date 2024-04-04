using Telegram.Bot.Types;
using TG_Bot_MVC.DataClasses;
using TG_Bot_MVC.Keyboards;

namespace TG_Bot_MVC.Controller
{
    internal class StartController(BotView botView, LocalAPI localAPI, DDoSData dDoSData) : BaseController(botView, localAPI, dDoSData)
    {
        private ConfigWorker _configWorker = new();

        public override async Task ActiveController(Update update, UserUpdate userUpdate, CancellationToken cancellationToken)
        {
            string response = _configWorker.GetHelloMessage();
            var inlineBoard = new Inline();
            await _view.SendCommandResponse(
                update.Message.Chat.Id,
                response,
                inlineBoard.inlineKeyboardSetting,
                cancellationToken);
        }
    }
}
