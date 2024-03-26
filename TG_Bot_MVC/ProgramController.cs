using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Passport;
using TG_Bot_MVC.DataClasses;
using TG_Bot_MVC.Handlers;

namespace TG_Bot_MVC
{
    internal class ProgramController
    {
        private readonly BotVeiw _botVeiw;
        private readonly ConfigWorker _configWorker;
        private readonly LocalAPI _localAPI;
        private readonly DDoSData _dDoSData;

        public ProgramController(BotVeiw botVeiw, LibraryContext context)
        {
            _botVeiw = botVeiw;
            _configWorker = new ConfigWorker();
            _localAPI = new LocalAPI(context);
            _dDoSData = new DDoSData();
        }

        internal async Task BotController(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long chatId = 0;
            if (update.CallbackQuery is not { } messageCallbackQuery)
            {
                messageCallbackQuery = null;
                chatId = update.Message.Chat.Id;
            }
                
            if (update.Message is not { } message)
            {
                message = update.CallbackQuery.Message;
                chatId = update.CallbackQuery.Message.Chat.Id;
            }
            if (message == null && messageCallbackQuery == null)
                return;

            // Init ALL BaseUpdateHandlers
            BaseUpdateHandler dataHandler = new DateHandler(null, _localAPI);
            BaseUpdateHandler dataToRequestHandler = new DateToRequestHandler(dataHandler, _localAPI);
            BaseUpdateHandler dDoSHandler = new DDoSHandler(dataToRequestHandler, _localAPI, _dDoSData);
            BaseUpdateHandler handler = new MainHandler(dDoSHandler, _localAPI);  // MAIN Handler

            var userUpdate = new UserUpdate()
            {
                UserMessage = message,
                DateToRequest = DateTime.Today
            };

            if (handler.Active(userUpdate))
            {

                if (messageCallbackQuery != null)
                {
                    var callbackHandler = new CallbackQuryHandler(_localAPI, messageCallbackQuery.Data, userUpdate);
                    var response = callbackHandler.Active();
                    await _botVeiw.EditInlineMessage(chatId, messageCallbackQuery.Message.MessageId, response, cancellationToken);
                }
                else if (message.Text == "/start")
                {
                    string response = _configWorker.GetHelloMessage();
                    await _botVeiw.SendCommandResponse(chatId, response, cancellationToken);
                }
                else if (message.Text.StartsWith("/"))
                {
                    await _botVeiw.SendCommandResponse(chatId, message.Text, cancellationToken);
                }
                else
                {
                    string response = _localAPI.GetCorrectSchedule(
                        _localAPI.GetFullUser(message.Chat.Id).Setting.GroupId,
                        (int)userUpdate.DateToRequest.DayOfWeek)
                        .SerializeDataLessons;
                    await _botVeiw.SendDefaultResponse(chatId, response, cancellationToken);
                }
            }
        }

        internal Task ErrorBotController(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
