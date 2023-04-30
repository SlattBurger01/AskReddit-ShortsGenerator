using OpenQA.Selenium;
using Selenium.Extensions;
using System.Text.RegularExpressions;

namespace ProjectCarrot
{
    public static class Utils
    {
        public static int GetWordCount(string input)
        {
            string[] words = input.Split(' ');
            return words.Length;
        }

        public static string RemoveHyperlinks(string input)
        {
            string pattern = @"http[^\s]+";

            return Regex.Replace(input, pattern, string.Empty);
        }
    }
}