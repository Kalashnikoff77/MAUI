using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Data.Extensions
{
    public static class GlobalExtension
    {
        /// <summary>
        /// Глубокое копирование через JSON
        /// </summary>
        public static T? DeepCopy<T>(this object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<T>(json)!;
            return result;
        }


        /// <summary>
        /// Замена \n\r на <br>
        /// </summary>
        public static MarkupString ReplaceNewLineWithBR(this string text) =>
            new MarkupString(Regex.Replace(text, @"\r\n?|\n", "<br />"));


        /// <summary>
        /// Удаление пустых линий
        /// </summary>
        public static string RemoveEmptyLines(this string text) =>
            Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);


        /// <summary>
        /// Урезает коротку строку до определённой длины
        /// </summary>
        /// <param name="text">Сам текст</param>
        /// <param name="length">Максимальная длина</param>
        /// <param name="finish">Добавить в конец обрезаемого текста</param>
        /// <returns>Это пример обреза...</returns>
        public static string? ToShortString(this string text, int length, string finish = "...")
        {
            var sb = new StringBuilder(text);
            if (text != null && text.Length > length)
            {
                sb.Clear().Append(text.Substring(0, length));
                if (!string.IsNullOrEmpty(finish))
                    sb.Append(finish);
            }
            return sb.ToString();
        }
    }
}
