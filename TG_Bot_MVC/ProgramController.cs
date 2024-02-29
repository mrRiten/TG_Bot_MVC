using Org.BouncyCastle.Pqc.Crypto;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    internal class ProgramController
    {
        private readonly BotVeiw _botVeiw;
        private readonly ConfigWorker _configWorker;
        private readonly LocalAPI _localAPI;

        public ProgramController(BotVeiw botVeiw)
        {
            _botVeiw = botVeiw;
            _configWorker = new ConfigWorker();
            _localAPI = new LocalAPI(new LibraryContext());
        }

        internal async Task BotController(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            var chatId = update.Message.Chat.Id;

            // Init ALL IMessageHandler
            IUpdateHandler testHandler = new TestHandler(null, _localAPI);  // MAIN Handler
            IUpdateHandler handler = new MainHandler(testHandler, _localAPI);  // for test

            if (handler.Active(update.Message))
            {
                if (messageText.StartsWith("/"))
                {
                    await _botVeiw.SendCommandResponse(chatId, messageText, cancellationToken);
                }
                else
                {
                    await _botVeiw.SendDefaultResponse(chatId, messageText, cancellationToken);
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
