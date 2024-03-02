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
            //"https://menu.sttec.yar.ru/timetable/rasp_first.html";

            HtmlWeb web = new();
            HtmlDocument doc = web.Load(url);

            try
            {
                HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

                // Get the denominator or numerator from an HTML document
                string weekOfSchedule = GetWeekOfSchedule(doc);

                if (table != null)
                {
                    var groupData = ParseScheduleTable(table);

                    string[] json = SerializeDictToArrString(groupData);

                    Logger.LogInfo("Parsing!");

                    WriteToDatabase(groupData, json, weekOfSchedule);

                    Logger.LogInfo("Writed to database!");
                }
                else
                    throw new NoTableException("Таблица не найдена.");
            }
            catch (NoTableException ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message}");
            }
            catch (LessThanThreeCellsException ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message}");
            }
            catch (NoBracketsException ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message}");
            }
            catch (InvalidFormatException ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message} Значение номера пары: {ex.CurrentValue}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Произошла неизвестная ошибка в Parser: {ex.Message}");
            }
        }
        //Maybe remove
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
                throw new NoBracketsException("Скобки для получения знаменателя или числителя не найдены.");
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
                    throw new LessThanThreeCellsException("Ячеек в таблице меньше 3.");
            }
            return groupData;
        }

        private static int[] ValidateNumbersReplacementLessons(string numbersReplacementLessons)
        {
            var ValidNumsList = new List<int>();

            if (numbersReplacementLessons.Contains(','))
            {
                string[] temp = numbersReplacementLessons.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    ValidNumsList.Add(int.Parse(temp[i]));
                }
            }
            else if (numbersReplacementLessons.Contains('-'))
            {
                string[] temp = numbersReplacementLessons.Split('-');
                for (int i = int.Parse(temp[0]); i <= int.Parse(temp[temp.Length - 1]); i++)
                {
                    ValidNumsList.Add(i);
                }
            }
            else if (numbersReplacementLessons.Contains('.'))
            {
                double temp = double.Parse(numbersReplacementLessons, CultureInfo.InvariantCulture);
                if (temp <= 9.10)
                    ValidNumsList.Add(0);
                else if (temp <= 10.50)
                    ValidNumsList.Add(1);
                else if (temp <= 12.30)
                    ValidNumsList.Add(2);
                else if (temp <= 14.50)
                    ValidNumsList.Add(3);
                else if (temp <= 16.35)
                    ValidNumsList.Add(4);
                else if (temp <= 18.35)
                    ValidNumsList.Add(5);
                else
                    ValidNumsList.Add(6);
            }
            else if (numbersReplacementLessons.Length == 1)
                ValidNumsList.Add(int.Parse(numbersReplacementLessons));
            else
                throw new InvalidFormatException("Неверный формат номера пары в таблице.", numbersReplacementLessons);

            return ValidNumsList.ToArray();
        }

        private static string[] SerializeDictToArrString(Dictionary<string, Dictionary<int, string>> rowDataDict)
        {
            var json = new List<string>();
            foreach (var item in rowDataDict)
            {
                json.Add(JsonConvert.SerializeObject(item.Value, Formatting.Indented));
            }
            return json.ToArray();
        }
        private static void WriteToDatabase(Dictionary<string, Dictionary<int, string>> groupData, string[] json, string weekOfSchedule)
        {

            var context = new LibraryContext();
            var localAPI = new LocalAPI(context);

            DateTime today = DateTime.Today;

            Logger.LogDebug($"Замены перед записью в БД: ");
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
    class NoTableException(string message) : Exception(message) { }
    class NoBracketsException(string message) : Exception(message) { }
    class LessThanThreeCellsException(string message) : Exception(message) { }
    class InvalidFormatException(string message, string currentValue) : ArgumentException(message) { public string CurrentValue { get; } = currentValue; }
}