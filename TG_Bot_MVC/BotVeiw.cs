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
                InlineKeyboardButton.WithCallbackData(text: "Student", callbackData: "SStudent"),
                InlineKeyboardButton.WithCallbackData(text: "Teacher", callbackData: "STeacher"),
            }
            
        });


        public async Task EditInlineMessage(long IdChat, int IdMes, string response, CancellationToken cancellationToken)
        {

            await _bot.EditMessageTextAsync(IdChat, IdMes, response, replyMarkup: null);
            await _bot.SendTextMessageAsync(
                chatId: IdChat,
                text: "Выбеите действие",
                replyMarkup: defaultKeyboard,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

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
                text: response,
                replyMarkup: inlineKeyboardSetting,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

    }
}
