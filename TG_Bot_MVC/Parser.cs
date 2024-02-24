using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace TG_Bot_MVC
{
    internal static class Parser
    {
        public static void MainParse()
        {
            // URL, откуда будем загружать замены
            string url = "https://menu.sttec.yar.ru/timetable/rasp_first.html";

            try
            {
                // Создаем объект для загрузки HTML-страницы
                HtmlWeb web = new HtmlWeb();
                // Загружаем HTML-документ по указанному URL
                HtmlDocument doc = web.Load(url);

                // Получение знаменателя или числителя из HTML-документа
                string weekOfSchedule = GetWeekOfSchedule(doc);
                Console.WriteLine(weekOfSchedule);

                // Получение таблицы замен из HTML-документа
                HtmlNode table = doc.DocumentNode.SelectSingleNode("//table");

                if (table != null)
                {
                    // Парсинг таблицы замен и формирование словаря данных о заменах в расписании
                    var groupData = ParseScheduleTable(table);

                    // Запись данных о расписании в JSON файлы
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
            // Выбираем все элементы div в HTML-документе
            var divElements = doc.DocumentNode.SelectNodes("//div");

            // Ищем текст в скобках с помощью регулярного выражения
            Regex regex = new Regex(@"\((.*?)\)");
            Match match = regex.Match(divElements[3].InnerText);

            if (match.Success)
            {
                // Извлекаем текст из скобок из группы с индексом 1
                return match.Groups[1].Value;
            }
            else
                throw new Exception("Скобки не найдены");
        }

        private static Dictionary<string, Dictionary<int, string>> ParseScheduleTable(HtmlNode table)
        {
            // Создаем словарь, где ключом является название группы, а значением словарь пар номеров замен и соответствующих данных о заменах
            var groupData = new Dictionary<string, Dictionary<int, string>>();

            // Получаем коллекцию строк таблицы
            HtmlNodeCollection rows = table.SelectNodes(".//tr");

            // Начинаем с 1, чтобы пропустить заголовок таблицы
            for (int i = 1; i < rows.Count; i++)
            {
                // Получаем ячейки текущей строки
                HtmlNodeCollection cells = rows[i].SelectNodes(".//td");

                // Проверяем, что в строке есть как минимум три ячейки
                if (cells != null && cells.Count > 2)
                {
                    // Проверяем, что во второй и третьей ячейках есть данные (название группы и номера замен)
                    if (!string.IsNullOrEmpty(cells[1].InnerText) && !string.IsNullOrEmpty(cells[2].InnerText))
                    {
                        // Получаем название группы, приводим его к верхнему регистру и убираем лишние пробелы
                        string group = cells[1].InnerText.ToUpper().Trim();

                        // Получение номеров замен и соответствующих данных о заменах
                        string NumbersReplacementLessons = cells[2].InnerText;
                        string rowData = $"{cells[4].InnerText} {cells[5].InnerText}";

                        // Валидация номеров замен
                        int[] keys = ValidateNumbersReplacementLessons(NumbersReplacementLessons);

                        // Если группа еще не добавлена в словарь, добавляем ее
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

                        // Записываем данные о заменах в словарь
                        foreach (int key in keys)
                        {
                            value[key] = rowData;
                        }
                    }
                }
                else
                    throw new Exception("Ячеек меньше 3");
            }
            return groupData;
        }

        // Валидация номеров замен
        private static int[] ValidateNumbersReplacementLessons(string NumbersReplacementLessons)
        {
            var list = new List<int>();

            if (NumbersReplacementLessons.Contains(','))
            {
                string[] temp = NumbersReplacementLessons.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    list.Add(int.Parse(temp[i]));
                }
            }
            else if (NumbersReplacementLessons.Contains('-'))
            {
                string[] temp = NumbersReplacementLessons.Split('-');
                for (int i = int.Parse(temp[0]); i <= int.Parse(temp[temp.Length - 1]); i++)
                {
                    list.Add(i);
                }
            }
            else
                list.Add(int.Parse(NumbersReplacementLessons));

            return list.ToArray();
        }

        // Запись данных о расписании в файлы JSON
        private static void WriteScheduleDataToJson(Dictionary<string, Dictionary<int, string>> groupData)
        {
            foreach (var kvp in groupData)
            {
                string group = kvp.Key;
                var rowDataDict = kvp.Value;

                if (rowDataDict.Count > 0)
                {
                    // Сериализация данных в JSON формат и запись в файл
                    string json = JsonConvert.SerializeObject(rowDataDict, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText($"row_data_{group}.json", json);
                }
                else
                    throw new Exception($"Для группы {group} не найдены строки, удовлетворяющие условиям.");
            }
        }
    }
}
