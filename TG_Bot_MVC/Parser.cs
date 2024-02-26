using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace TG_Bot_MVC
{
    internal static class Parser
    {
        public static string Group { get; private set; }
        public static string WeekOfSchedule { get; private set; }
        public static string Json { get; private set; }

        public static void MainParse()
        {
            string url = "https://menu.sttec.yar.ru/timetable/rasp_first.html";

            try
            {
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                // Get the denominator or numerator from an HTML document
                WeekOfSchedule = GetWeekOfSchedule(doc);

                HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

                if (table != null)
                {
                    // Parse the replacement table and creating a dictionary of replacement data in the schedule
                    var groupData = ParseScheduleTable(table);

                    WriteScheduleDataToJson(groupData);

                    Console.WriteLine("Данные о расписании успешно записаны в файлы JSON.");
                }
                else
                    throw new Exception("Таблица не найдена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private static string GetWeekOfSchedule(HtmlDocument doc)
        {
            var divElements = doc.DocumentNode.SelectNodes("//div");

            // Look for the text in parentheses using a regular expression
            Regex regex = new Regex(@"\((.*?)\)");
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
            // Creat a dictionary where the key is the name of the group, and the value is a dictionary of pairs of replacement numbers and corresponding replacement data
            var groupData = new Dictionary<string, Dictionary<int, string>>();

            HtmlNodeCollection rows = table.SelectNodes(".//tr");

            // Start with 1 to skip the table header
            for (int i = 1; i < rows.Count; i++)
            {
                HtmlNodeCollection cells = rows[i].SelectNodes(".//td");

                if (cells != null && cells.Count > 2)
                {
                    if (!string.IsNullOrEmpty(cells[1].InnerText) && !string.IsNullOrEmpty(cells[2].InnerText))
                    {
                        Group = cells[1].InnerText.ToUpper().Trim();
                        string numbersReplacementLessons = cells[2].InnerText;
                        string rowData = $"{cells[4].InnerText} {cells[5].InnerText}";

                        int[] keys = ValidateNumbersReplacementLessons(numbersReplacementLessons);

                        if (!groupData.TryGetValue(Group, out Dictionary<int, string>? value))
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
                            groupData[Group] = value;
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
            else
                list.Add(int.Parse(numbersReplacementLessons));

            return list.ToArray();
        }

        private static void WriteScheduleDataToJson(Dictionary<string, Dictionary<int, string>> groupData)
        {
            foreach (var kvp in groupData)
            {
                string group = kvp.Key;
                var rowDataDict = kvp.Value;

                if (rowDataDict.Count > 0)
                {
                    Json = Serializer.SerializeJson(rowDataDict);
                    File.WriteAllText($"{group}.json", Json);
                }
                else
                    throw new Exception($"Для группы {group} не найдены строки, удовлетворяющие условиям.");
            }
        }
    }
}
