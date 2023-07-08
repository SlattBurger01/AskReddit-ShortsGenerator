using System.Text;
using System.Text.RegularExpressions;

namespace ProjectCarrot
{
    public static class TextsManager
    {
        public static string GetFixedText(string text)
        {
            string fText = text.Replace('"'.ToString(), string.Empty); // so it does not mess up args for python text to speech

            fText = fText.Replace("!?!", "?");
            fText = fText.Replace("?!?", "?");
            fText = fText.Replace("”", string.Empty);
            fText = fText.Replace("“", string.Empty);

            fText = Regex.Replace(fText, @"\p{Cs}", ""); // Removes emojis from text

            fText = ReduceDots(fText); // Reduce them before spaces are added

            fText = AddSpacesAfterDots(fText);

            fText = RemoveCommasAfterEndsOfSentences(fText);

            return ExplicitWordHandler.ReplaceExplicitWords(fText);
        }

        private static string RemoveCommasAfterEndsOfSentences(string t)
        {
            string pattern = @"([?!.]),";
            string replacementP = "$1";

            return Regex.Replace(t, pattern, replacementP);
        }

        private static string ReduceDots(string text) => ReduceChar(text, '.');

        private static string ReduceChar(string s, char ch) => Regex.Replace(s, @$"\{ch}+", $"{ch}");

        public static string AddSpacesAfterDots(string input)
        {
            string pattern = @"\.(?! )";
            string replacement = ". ";

            string output = Regex.Replace(input, pattern, replacement);

            return output;
        }
    }

    public static class ExplicitWordHandler
    {
        private static readonly (string, string)[] explicitWords = { }; // (explicit word, it's replacement)
        //private static readonly (string, string)[] specificChars = { };

        public static string ReplaceExplicitWords(string input)
        {
            StringBuilder builder = new StringBuilder(input);

            Replace(ref builder, explicitWords);

            return builder.ToString();
        }

        private static void Replace(ref StringBuilder builder, (string, string)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                (string, string) v = data[i];
                builder.Replace(v.Item1, v.Item2);
            }
        }
    }
}