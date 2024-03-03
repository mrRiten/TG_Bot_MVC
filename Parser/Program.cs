using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using TG_Bot_MVC;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParserHTML.MainParse("https://menu.sttec.yar.ru/timetable/rasp_first.html");
            ParserHTML.MainParse("https://menu.sttec.yar.ru/timetable/rasp_second.html");
        }
    }
}