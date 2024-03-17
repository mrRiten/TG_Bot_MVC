using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG_Bot_MVC;

namespace Parser
{
    internal class ParserObserverCreator : IObserver
    {
        public void Update()
        {
            Logger.LogInfo("Call ParserObserverCreator");
        }
    }

    internal class ParserObserverDeletor : IObserver
    {
        public void Update()
        {
            Logger.LogInfo("Call ParserObserverDelitor");
        }
    }
}
