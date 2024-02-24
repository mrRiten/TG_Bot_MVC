using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TG_Bot_MVC
{
    internal static class Parser
    {
        public static void ParseHTML(){

            string url = "https://menu.sttec.yar.ru/timetable/rasp_first.html";
            string group = "ис1-21";

            Console.WriteLine($"Группа: {group}");
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                var divElements = doc.DocumentNode.SelectNodes("//div");
                string weekOfSchedule = divElements[3].InnerText;
                int IdWoS = FindIdWeekOfSchedule(weekOfSchedule);
                Console.WriteLine($"{weekOfSchedule} - {IdWoS}");

                HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

                if (table != null)
                {
                    var rowDataDict = new Dictionary<int, string>() 
                    {
                        { 0, null },
                        { 1, null },
                        { 2, null },
                        { 3, null },
                        { 4, null },
                        { 5, null },
                        { 6, null },
                    };

                    HtmlNodeCollection rows = table.SelectNodes(".//tr");

                    foreach (HtmlNode row in rows)
                    {
                        HtmlNodeCollection cells = row.SelectNodes(".//td");

                        if (cells != null && cells.Count > 1)
                        {
                            if (cells[1].InnerText.Contains(group.ToUpper()))
                            {
                                string NumbersReplacementLessons = cells[2].InnerText;
                                string rowData = $"{cells[4].InnerText} {cells[5].InnerText}";

                                int[] keys = ValidateNumbersReplacementLessons(NumbersReplacementLessons);
                                foreach (int key in keys)
                                {
                                    rowDataDict[key] = rowData;
                                }
                            }
                        }
                        else
                            throw new Exception("Не найден 2 столбец в строке.");
                    }

                    if (rowDataDict.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(rowDataDict, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText("row_data.json", json);

                        Console.WriteLine("Данные строк записаны в файл row_data.json");
                    }
                    else
                        throw new Exception("Не найдены строки, удовлетворяющие условиям.");
                }
                else
                    throw new Exception("Таблица не найдена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
        private static int[] ValidateNumbersReplacementLessons(string NumbersReplacementLessons)
        {
            var arrList = new List<int>();

            if (NumbersReplacementLessons.Contains(','))
            {
                string[] temp = NumbersReplacementLessons.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    arrList.Add(int.Parse(temp[i]));
                }
            }
            else if (NumbersReplacementLessons.Contains('-'))
            {
                string[] temp = NumbersReplacementLessons.Split('-');
                for (int i = int.Parse(temp[0]); i <= int.Parse(temp[temp.Length - 1]); i++)
                {
                    arrList.Add(i);
                }
            }
            else 
                arrList.Add(int.Parse(NumbersReplacementLessons));

            return arrList.ToArray();
        }
        private static int FindIdWeekOfSchedule(string weekOfSchedule)
        {




            return 1;
        }
    }
}
