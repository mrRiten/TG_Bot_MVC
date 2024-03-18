using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using TG_Bot_MVC;

namespace Parser
{
    internal class ParserHTML : ISubject
    {
        public static string WeekOfSchedule;

        private List<IObserver> ObserverList;

        public ParserHTML(IObserver observer)
        {
            ObserverList = new List<IObserver>();
            AddObserver(observer);
        }

        public void Notify()
        {
            foreach (var observer in ObserverList)
            {
                observer.Update();
            }
        }

        public void AddObserver(IObserver observer)
        {
            ObserverList.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            ObserverList.Remove(observer);
        }

        public void MainParse(string[] url)
        {
            HtmlWeb web = new();
            HtmlDocument document1 = web.Load(url[0]);
            HtmlDocument document2 = web.Load(url[1]);

            try
            {
                HtmlNode table1 = document1.DocumentNode.SelectSingleNode("//table");
                HtmlNode table2 = document2.DocumentNode.SelectSingleNode("//table");

                // Get the denominator or numerator from an HTML document
                WeekOfSchedule = GetWeekOfSchedule(document1);

                if (table1 != null)
                {
                    Dictionary<string, Dictionary<int, string>>[] groupDataArr = [ParseScheduleTable(table1), ParseScheduleTable(table2)];

                    string[] json = SerializeDictToArrString(groupDataArr);

                    Logger.LogInfo("Parsing!");

                    WriteToDatabase(groupDataArr, json, WeekOfSchedule);

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

            Notify();
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
        private static string[] SerializeDictToArrString(Dictionary<string, Dictionary<int, string>>[] groupDataArr)
        {
            var json = new List<string>();
            foreach (var dataDict in groupDataArr)
            {
                foreach (var item in dataDict)
                {
                    json.Add(JsonConvert.SerializeObject(item.Value, Formatting.Indented));
                }
            }
            return json.ToArray();
        }
        private static void WriteToDatabase(Dictionary<string, Dictionary<int, string>>[] groupDataArr, string[] json, string weekOfSchedule)
        {
            LibraryContext context = new(true); // Debug mode = true! TODO: add handling debug mode with Main args
            var localAPI = new LocalAPI(context);

            localAPI.DelReplasementLessons((int)DateTime.Today.DayOfWeek);
            Logger.LogDebug($"Замены перед записью в БД: ");
            int i = 0;
            foreach (var groupData in groupDataArr)
            {
                foreach (var group in groupData.Keys)
                {
                    Logger.LogDebug($"{group} - {json[i]}");
                    localAPI.AddReplasementLesson(
                        localAPI.TryGetGroupId(group),
                        localAPI.GetWeekOfScheduleId(weekOfSchedule),
                        json[i],
                        (int)DateTime.Today.DayOfWeek
                    );
                    i++;
                }
            }

        }
    }
    class NoTableException(string message) : Exception(message) { }
    class NoBracketsException(string message) : Exception(message) { }
    class LessThanThreeCellsException(string message) : Exception(message) { }
    class InvalidFormatException(string message, string currentValue) : ArgumentException(message) { public string CurrentValue { get; } = currentValue; }
}
