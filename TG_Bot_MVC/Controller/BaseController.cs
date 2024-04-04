using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Controller
{
    public abstract class BaseController
    {
        protected readonly LocalAPI _localAPI;
        protected readonly BotView _view;
        protected readonly DDoSData _dDoSData;

        protected BaseController(BotView botView, LocalAPI localAPI, DDoSData dDoSData)
        {
            _view = botView;
            _localAPI = localAPI;
            _dDoSData = dDoSData;
        }

        public abstract Task ActiveController(Telegram.Bot.Types.Update update, UserUpdate userUpdate, CancellationToken cancellationToken);

    }
}
