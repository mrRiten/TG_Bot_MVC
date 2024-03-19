using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Handlers
{
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
}
