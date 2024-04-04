using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TG_Bot_MVC.Controller;
using TG_Bot_MVC.DataClasses;
using TG_Bot_MVC.Handlers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TG_Bot_MVC
{
    internal class ProgramController
    {
        private readonly BotView _botVeiw;
        private readonly ConfigWorker _configWorker;
        private readonly LocalAPI _localAPI;
        private readonly DDoSData _dDoSData;
        private Dictionary<string, BaseController> _controllerDict;

        private UpdateControllerInfo _controllerInfo = new();

        public ProgramController(BotView botVeiw, LibraryContext context)
        {
            _botVeiw = botVeiw;
            _configWorker = new ConfigWorker();
            _localAPI = new LocalAPI(context);
            _dDoSData = new DDoSData();
            _controllerDict = new Dictionary<string, BaseController>
            {
                { "Command", new CommandController(_botVeiw, _localAPI, _dDoSData) },
                { "Start", new StartController(_botVeiw, _localAPI, _dDoSData) },
                { "Schedule", new ScheduleController(_botVeiw, _localAPI, _dDoSData) },
                { "Callback", new CallbackController(_botVeiw, _localAPI, _dDoSData) },
            };
        }

        internal async Task BotController(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery is not { } messageCallbackQuery)
            {
                messageCallbackQuery = null;
            }

            if (update.Message is not { } message)
            {
                message = update.CallbackQuery.Message;
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
                _controllerInfo.Update = update;
                _controllerInfo.UserUpdate = userUpdate;
                _controllerInfo.CancellationToken = cancellationToken;
                if (messageCallbackQuery != null)
                {
                    await CallController("Callback", _controllerInfo);
                }
                else if (message.Text.StartsWith("/start"))
                {
                    await CallController("Start", _controllerInfo);
                }
                else if (message.Text.StartsWith("/"))
                {
                    await CallController("Command", _controllerInfo);
                }
                else
                {
                    await CallController("Schedule", _controllerInfo);
                }

            }
        }

        private async Task CallController(string controllerName, UpdateControllerInfo updateInfo)
        {
            await _controllerDict[controllerName].ActiveController(updateInfo.Update, updateInfo.UserUpdate, updateInfo.CancellationToken);
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
