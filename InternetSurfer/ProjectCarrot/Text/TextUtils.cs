using System.Text.RegularExpressions;

namespace ProjectCarrot.Text
{
    public static class TextUtils
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