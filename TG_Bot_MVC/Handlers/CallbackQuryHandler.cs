using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Handlers
{
    internal class CallbackQuryHandler
    {
        private readonly LocalAPI _localAPI;
        private string messageCallback;
        private UserUpdate userUpdate;

        public CallbackQuryHandler(LocalAPI localAPI, string messageCallback, UserUpdate userUpdate)
        {
            _localAPI = localAPI;
            this.messageCallback = messageCallback;
            this.userUpdate = userUpdate;
        }
        
        public string Active()
        {
            if (messageCallback.StartsWith("S"))
            {
                return HandlingStatus();
            }
            return "Error";
        }

        private string HandlingStatus()
        {
            if (messageCallback == "SStudent")
            {
                _localAPI.SetStatus(userUpdate.UserMessage.Chat.Id, 1);
                return "Ваш статус изменён на Student";
            }
            else if (messageCallback == "STeacher")
            {
                _localAPI.SetStatus(userUpdate.UserMessage.Chat.Id, 2);
                return "Ваш статус изменён на Teacher";
            }
            return "Error";
        }

    }
}
