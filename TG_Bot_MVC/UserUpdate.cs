using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TG_Bot_MVC
{
    public class UserUpdate
    {
        public Message UserMessage { get; set; }
        public DateTime DateToRequest { get; set; }
    }
}
