using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TG_Bot_MVC.Keyboards;

namespace TG_Bot_MVC
{
    public class BotView
    {
        public TelegramBotClient _bot { get; set; }

        readonly ReplyKeyboardMarkup defaultKeyboard = new(new[]
            {
                new KeyboardButton[] { "📑 Расписание" },
                ["⬅️ Предыдущее", "Следующее ➡️"],
                ["⚙️ Настройки"]
            })
        {
            ResizeKeyboard = true
        };

        public async Task EditInlineMessage(long IdChat, int IdMes, string response)
        {
            await _bot.EditMessageTextAsync(IdChat, IdMes, response, replyMarkup: null);
            //await _bot.SendTextMessageAsync(
            //    chatId: IdChat,
            //    text: newResponse,
            //    replyMarkup: inlineKeyboard,
            //    parseMode: ParseMode.Markdown,
            //    cancellationToken: cancellationToken);
        }

        public async Task SendDefaultResponse(long IdChat, string response, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received a '{response}' message in chat {IdChat}.");

            await _bot.SendTextMessageAsync(
                chatId: IdChat,
                text: response,
                replyMarkup: defaultKeyboard,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        public async Task SendCommandResponse(long IdChat, string response, InlineKeyboardMarkup inlineKeyboard, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received a '{response}' message in chat {IdChat}.");

            await _bot.SendTextMessageAsync(
                chatId: IdChat,
                text: response,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

    }
}
