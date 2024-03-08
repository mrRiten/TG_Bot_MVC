using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    public abstract class BaseUpdateHandler(LocalAPI localAPI)
    {
        public readonly LocalAPI localAPI = localAPI;
        public readonly BaseUpdateHandler? nextHandler;

        public abstract bool Active(UserUpdate userUpdate);

        public bool? CallNextHandler(UserUpdate userUpdate)
        {
            return Active(userUpdate);
        }
    }

    public class MainHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            var user = localAPI.TryGetUser(userUpdate.UserMessage.Chat.Id, userUpdate.UserMessage.Chat.FirstName);

            if (user.IsBanned == false)
            {
                return nextHandler?.CallNextHandler(userUpdate) ?? true;
            }
            return false;
        }
    }

    public class DDoSHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI, DDoSData dDoSData) : BaseUpdateHandler(localAPI)
    {
        public readonly DDoSData dDoSData = dDoSData;
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            var userId = userUpdate.UserMessage.Chat.Id;
            DateTime currentTime = DateTime.Now;
            if (dDoSData.dictUserDate.ContainsKey(userId))
            {
                TimeSpan time_elapsed = currentTime - dDoSData.dictUserDate[userId];
                if (time_elapsed < dDoSData.timeLimit)
                {
                    Console.Out.WriteLine($"{dDoSData.dictUserDate[userId]}");
                    if (dDoSData.dictUserDate.ContainsKey(userId))
                    {
                        if (dDoSData.dictUserWarning.ContainsKey(userId))
                        {
                            if (dDoSData.dictUserWarning[userId] < 3)
                            {
                                userUpdate.UserMessage.Text = "spam";
                                dDoSData.dictUserWarning[userId] += 1;
                                Console.Out.WriteLine($"for user {userId} - DDoS - detected!");
                            }
                            else
                            {
                                localAPI.SetBan(userId, true);
                                Console.Out.WriteLine($"for user {userId} - DDoS - Banned!");
                                return false;
                            }
                        }
                        else
                        {
                            dDoSData.dictUserWarning[userId] = 1;
                        }
                    }
                }
            }
            dDoSData.dictUserDate[userId] = currentTime;
            return nextHandler?.CallNextHandler(userUpdate) ?? true;
        }
    }

    public class DateToRequestHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            if (userUpdate.UserMessage.Text == "⬅️ Предыдущее")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(-1);
            }
            else if (userUpdate.UserMessage.Text == "Следующее ➡️")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(1);
            }
            return nextHandler?.CallNextHandler(userUpdate) ?? true;
        }
    }


    public class DateHandler(BaseUpdateHandler? nextHandler, LocalAPI localAPI) : BaseUpdateHandler(localAPI)
    {
        public new readonly BaseUpdateHandler? nextHandler = nextHandler;

        public override bool Active(UserUpdate userUpdate)
        {
            if (userUpdate.DateToRequest.DayOfWeek == DayOfWeek.Sunday && userUpdate.UserMessage.Text == "Следующее ➡️")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(1);
            }
            else if (userUpdate.DateToRequest.DayOfWeek == DayOfWeek.Sunday && userUpdate.UserMessage.Text == "⬅️ Предыдущее")
            {
                userUpdate.DateToRequest = userUpdate.DateToRequest.AddDays(-1);
            }
            return nextHandler?.CallNextHandler(userUpdate) ?? true;
        }
    }

}
