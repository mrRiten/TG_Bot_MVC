using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG_Bot_MVC
{
    public class DDoSData
    {
        public Dictionary<long, DateTime> dictUserDate = new();
        public Dictionary<long, int> dictUserWarning = new();
        public TimeSpan timeLimit = new TimeSpan(0, 0, 1);
    }
}
