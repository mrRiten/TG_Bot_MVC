using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot_MVC.Keyboards
{
    public class Inline
    {
        public readonly InlineKeyboardMarkup inlineKeyboardSetting = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Student", callbackData: "SStudent"),
                InlineKeyboardButton.WithCallbackData(text: "Teacher", callbackData: "STeacher"),
            }

        });

        public readonly InlineKeyboardMarkup inlineKeyboardDepartment = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "ОИТ", callbackData: "DOIT"),
                InlineKeyboardButton.WithCallbackData(text: "Later", callbackData: "DLater"),
                InlineKeyboardButton.WithCallbackData(text: "Later", callbackData: "DLater"),
                InlineKeyboardButton.WithCallbackData(text: "Later", callbackData: "DLater"),
            }

        });
    }
}
