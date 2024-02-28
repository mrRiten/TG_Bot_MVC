using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    internal class ProgramController
    {
        private readonly BotVeiw _botVeiw;
        private readonly ConfigWorker _configWorker;

        public ProgramController(BotVeiw botVeiw)
        {
            _botVeiw = botVeiw;
            _configWorker = new ConfigWorker();
        }

        internal async Task BotController(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            await _botVeiw.SendDefaultResponse(chatId, messageText, cancellationToken);
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
