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
        public static DayOfWeek CurrentDayOfWeek;

        private List<IObserver> ObserverList;

        public ParserHTML(List<IObserver> observer)
        {
            ObserverList = observer;
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

            HtmlNode table1 = document1.DocumentNode.SelectSingleNode("//table");
            HtmlNode table2 = document2.DocumentNode.SelectSingleNode("//table");

            if (table1 == null)
                throw new NoTable1Exception("Таблица первой смены не найдена.");
            if (table2 == null)
                throw new NoTable2Exception("Таблица второй смены не найдена.");

            try
            {
                WeekOfSchedule = GetWeekOfSchedule(document1);
                CurrentDayOfWeek = GetDayOfWeek(document1);

                Dictionary<string, Dictionary<int, string>>[] groupDataArr = [ParseScheduleTable(table1), ParseScheduleTable(table2)];

                string[] json = SerializeDictToArrString(groupDataArr);

                Logger.LogInfo("Parse completed");

                WriteToDatabase(groupDataArr, json);

                Logger.LogInfo("Replace schedule writed to database!");
            }
            catch (NoTable1Exception ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message}");
            }
            catch (NoTable2Exception ex)
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
            catch (DayOfWeekException ex)
            {
                Logger.LogWarning($"Произошло исключение в Parser: {ex.Message} Название дня недели: {ex.CurrentValue}");
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

                if (cells == null || cells.Count < 2) 
                    continue;


                string group = cells[1].InnerText;
                string numbersReplacementLesson = cells[2].InnerText;

                if (!string.IsNullOrEmpty(group) || !string.IsNullOrEmpty(numbersReplacementLesson))
                {
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

                    // timeReplacementLesson maybe empty. Example: 8.20 or 9.30
                    int[] keys = ValidateNumbersReplacementLessons(numbersReplacementLesson, out string timeReplacementLesson);

                    string rowData = $"{timeReplacementLesson}{cells[4].InnerText} - {cells[5].InnerText} (❗ замена)";

                    // Write data about substitutions in the dictionary
                    foreach (int key in keys)
                        value[key] = rowData;
                }
            }
            return groupData;
        }

        private static DayOfWeek GetDayOfWeek(HtmlDocument document)
        {
            HtmlNodeCollection divElements = document.DocumentNode.SelectNodes("//div");
            string[] strings = divElements[2].InnerText.Split(' ');
            string dayOfWeek = strings[strings.Length - 1];

            CultureInfo culture = new("ru-RU");
            string[] russiaDayNames = culture.DateTimeFormat.DayNames;

            for (int i = 0; i < russiaDayNames.Length; i++)
            {
                if (russiaDayNames[i].Equals(dayOfWeek, StringComparison.CurrentCultureIgnoreCase))
                    return (DayOfWeek)i;
            }

            throw new DayOfWeekException("Некорректное название дня недели.", dayOfWeek);
        }

        private static int[] ValidateNumbersReplacementLessons(string numbersReplacementLesson, out string timeReplacementLesson)
        {
            var validNumsList = new List<int>();
            timeReplacementLesson = "";

            if (numbersReplacementLesson.Contains(','))
            {
                string[] splitedNumbers = numbersReplacementLesson.Split(',');
                for (int i = 0; i < splitedNumbers.Length; i++)
                {
                    validNumsList.Add(int.Parse(splitedNumbers[i]));
                }
            }
            else if (numbersReplacementLesson.Contains('-'))
            {
                string[] splitedNumbers = numbersReplacementLesson.Split('-');
                for (int i = int.Parse(splitedNumbers[0]); i <= int.Parse(splitedNumbers[splitedNumbers.Length - 1]); i++)
                {
                    validNumsList.Add(i);
                }
            }
            else if (numbersReplacementLesson.Contains('.'))
            {
                double splitedNumbers = double.Parse(numbersReplacementLesson, CultureInfo.InvariantCulture);
                if (splitedNumbers <= 9.10)
                    validNumsList.Add(0);
                else if (splitedNumbers <= 10.50)
                    validNumsList.Add(1);
                else if (splitedNumbers <= 12.30)
                    validNumsList.Add(2);
                else if (splitedNumbers <= 14.50)
                    validNumsList.Add(3);
                else if (splitedNumbers <= 16.35)
                    validNumsList.Add(4);
                else if (splitedNumbers <= 18.35)
                    validNumsList.Add(5);
                else
                    validNumsList.Add(6);

                timeReplacementLesson = $"{numbersReplacementLesson} ";
            }
            else if (numbersReplacementLesson.Length == 1)
                validNumsList.Add(int.Parse(numbersReplacementLesson));
            else
                throw new InvalidFormatException("Неверный формат номера пары в таблице.", numbersReplacementLesson);

            return validNumsList.ToArray();
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
        private static void WriteToDatabase(Dictionary<string, Dictionary<int, string>>[] groupDataArr, string[] json)
        {
            LibraryContext context = new(true);
            LocalAPI localAPI = new(context);

            localAPI.DelReplasementLessons((int)CurrentDayOfWeek);
            Logger.LogDebug($"Замены перед записью в БД: ");
            int i = 0;
            foreach (var groupData in groupDataArr)
            {
                foreach (var group in groupData.Keys)
                {
                    Logger.LogDebug($"{group} - {json[i]}");
                    localAPI.AddReplasementLesson(
                        localAPI.TryGetGroupId(group),
                        localAPI.GetWeekOfScheduleId(WeekOfSchedule),
                        json[i],
                        (int)CurrentDayOfWeek
                    );
                    i++;
                }
            }
            Logger.LogDebug($"\n");
        }
    }
    class NoTable1Exception(string message) : Exception(message) { }
    class NoTable2Exception(string message) : Exception(message) { }
    class NoBracketsException(string message) : Exception(message) { }
    class InvalidFormatException(string message, string currentValue) : ArgumentException(message) { public string CurrentValue { get; } = currentValue; }
    class DayOfWeekException(string message, string currentValue) : ArgumentException(message) { public string CurrentValue { get; } = currentValue; }
}
