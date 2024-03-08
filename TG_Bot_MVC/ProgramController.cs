using Org.BouncyCastle.Pqc.Crypto;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Passport;

namespace TG_Bot_MVC
{
    internal class ProgramController
    {
        private readonly BotVeiw _botVeiw;
        private readonly ConfigWorker _configWorker;
        private readonly LocalAPI _localAPI;
        private readonly DDoSData _dDoSData;

        public ProgramController(BotVeiw botVeiw, bool isDebug)
        {
            _botVeiw = botVeiw;
            _configWorker = new ConfigWorker();
            _localAPI = new LocalAPI(new LibraryContext(isDebug));
            _dDoSData = new DDoSData();
        }

        internal async Task BotController(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //if (update.Message is not { } message)
            //    return;
            //if (message.Text is not { } messageText)
            //    return;

            string messageText = update.Message.Text;
            var message = update.Message;
            
            var chatId = update.Message.Chat.Id;

            // Init ALL BaseUpdateHandler
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
                if (messageText == "/start")
                {
                    string response = _configWorker.GetHelloMessage();
                    await _botVeiw.SendCommandResponse(chatId, response, cancellationToken);
                }
                else if (messageText.StartsWith("/"))
                {
                    await _botVeiw.SendCommandResponse(chatId, messageText, cancellationToken);
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
