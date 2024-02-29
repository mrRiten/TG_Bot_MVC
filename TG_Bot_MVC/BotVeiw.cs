using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot_MVC
{
    internal class BotVeiw
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

        readonly InlineKeyboardMarkup inlineKeyboardSetting = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Group", callbackData: "Group"),
                InlineKeyboardButton.WithCallbackData(text: "Departmetn", callbackData: "Departmetn"),
            }
        });

        public async Task SendDefaultResponse(long IdChat, string response, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received a '{response}' message in chat {IdChat}.");

            Message message = await _bot.SendTextMessageAsync(
                chatId: IdChat,
                text: response,
                replyMarkup: defaultKeyboard,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        public async Task SendCommandResponse(long IdChat, string response, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received a '{response}' message in chat {IdChat}.");

            Message message = await _bot.SendTextMessageAsync(
                chatId: IdChat,
                text: "Command!",
                replyMarkup: inlineKeyboardSetting,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

    }
}
