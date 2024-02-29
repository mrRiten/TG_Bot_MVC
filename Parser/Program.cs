using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TG_Bot_MVC;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //https://menu.sttec.yar.ru/timetable/rasp_first.html
            //https://menu.sttec.yar.ru/timetable/rasp_second.html
            const string url = "https://menu.sttec.yar.ru/timetable/rasp_first.html";

            try
            {
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                // Get the denominator or numerator from an HTML document
                string weekOfSchedule = GetWeekOfSchedule(doc);

                HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

                if (table != null)
                {
                    // Parse the replacement table and creating a dictionary of replacement data in the schedule
                    var groupData = ParseScheduleTable(table);

                    string[] json = SerializeDictToArrString(groupData);

                    Console.WriteLine("Parsing!");

                    WriteToDatabase(groupData, json, weekOfSchedule);

                    Console.WriteLine("Writed to database!");
                }
                else
                    throw new Exception("Таблица не найдена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Thread.Sleep(1800000);
                Main(args);
            }
        }

        private static string GetWeekOfSchedule(HtmlDocument doc)
        {
            var divElements = doc.DocumentNode.SelectNodes("//div");

            // Look for the text in parentheses using a regular expression
            Regex regex = new(@"\((.*?)\)");
            Match match = regex.Match(divElements[3].InnerText);

            if (match.Success)
            {
                // Extract the text from the brackets
                return match.Groups[1].Value;
            }
            else
                throw new Exception("Скобки не найдены");
        }

        private static Dictionary<string, Dictionary<int, string>> ParseScheduleTable(HtmlNode table)
        {
            // Create a dictionary where the key is the name of the group, and the value is a dictionary of pairs of replacement numbers and corresponding replacement data
            var groupData = new Dictionary<string, Dictionary<int, string>>();

            HtmlNodeCollection rows = table.SelectNodes(".//tr");

            // Start with 1 to skip the table header
            foreach (var row in rows.Skip(1))
            {
                HtmlNodeCollection cells = row.SelectNodes(".//td");

                if (cells != null && cells.Count > 2)
                {
                    if (!string.IsNullOrEmpty(cells[1].InnerText) && !string.IsNullOrEmpty(cells[2].InnerText))
                    {
                        string group = cells[1].InnerText.ToUpper().Trim();
                        string numbersReplacementLessons = cells[2].InnerText;
                        string rowData = $"{cells[4].InnerText} {cells[5].InnerText}";

                        int[] keys = ValidateNumbersReplacementLessons(numbersReplacementLessons);

                        if (!groupData.TryGetValue(group, out Dictionary<int, string>? value))
                        {
                            value = new Dictionary<int, string>()
                            {
                                { 0, null },
                                { 1, null },
                                { 2, null },
                                { 3, null },
                                { 4, null },
                                { 5, null },
                                { 6, null }
                            };
                            groupData[group] = value;
                        }

                        // Write data about substitutions in the dictionary
                        foreach (int key in keys)
                        {
                            value[key] = rowData;
                        }
                    }
                }
                else
                    throw new Exception("Ячеек в таблице меньше 3");
            }
            return groupData;
        }

        private static int[] ValidateNumbersReplacementLessons(string numbersReplacementLessons)
        {
            var list = new List<int>();

            if (numbersReplacementLessons.Contains(','))
            {
                string[] temp = numbersReplacementLessons.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    list.Add(int.Parse(temp[i]));
                }
            }
            else if (numbersReplacementLessons.Contains('-'))
            {
                string[] temp = numbersReplacementLessons.Split('-');
                for (int i = int.Parse(temp[0]); i <= int.Parse(temp[temp.Length - 1]); i++)
                {
                    list.Add(i);
                }
            }
            else if (numbersReplacementLessons.Length == 1)
                list.Add(int.Parse(numbersReplacementLessons));
            else
                throw new Exception("Неверный формат номера пары в таблице");
            return list.ToArray();
        }

        private static string[] SerializeDictToArrString(Dictionary<string, Dictionary<int, string>> rowDataDict)
        {
            var json = new List<string>();
            foreach (var item in rowDataDict)
            {
                json.Add(JsonConvert.SerializeObject(item.Value, Newtonsoft.Json.Formatting.Indented));
            }
            return json.ToArray();
        }
        private static void WriteToDatabase(Dictionary<string, Dictionary<int, string>> groupData, string[] json, string weekOfSchedule)
        {

            var context = new LibraryContext();
            var localAPI = new LocalAPI(context);

            DateTime today = DateTime.Today;

            int i = 0;
            foreach (var item in groupData.Keys)
            {
                Console.WriteLine($"{item} - {json[i]}");
                //localAPI.AddReplasementLesson(
                //    localAPI.TryGetGroupId(item),
                //    localAPI.GetWeekOfScheduleId(weekOfSchedule),
                //    json[i],
                //    today
                //);
                //i++;
            }
        }
    }
}