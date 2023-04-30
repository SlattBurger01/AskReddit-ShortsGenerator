using System.Text;

namespace ProjectCarrot
{
    public static class TextsManager
    {

    }

    public static class ExplicitWordHandler
    {
        private static readonly (string, string)[] explicitWords = { };
        private static readonly (string, string)[] specificChars = { };

        public static string ReplaceExplicitWords(string input)
        {
            StringBuilder builder = new StringBuilder(input);

            Replace(ref builder, explicitWords);
            Replace(ref builder, specificChars);

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