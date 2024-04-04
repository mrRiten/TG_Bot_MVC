using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Controller
{
    internal class ScheduleController(BotView botView, LocalAPI localAPI, DDoSData dDoSData) : BaseController(botView, localAPI, dDoSData)
    {

        public override async Task ActiveController(Telegram.Bot.Types.Update update, UserUpdate userUpdate, CancellationToken cancellationToken)
        {
            var message = update.Message;
            string response = _localAPI.GetCorrectSchedule(
                        _localAPI.GetFullUser(message.Chat.Id).Setting.GroupId,
                        (int)userUpdate.DateToRequest.DayOfWeek)
                        .SerializeDataLessons;
            await _view.SendDefaultResponse(message.Chat.Id, response, cancellationToken);
        }
    }
}
