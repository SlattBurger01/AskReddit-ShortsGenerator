namespace ProjectCarrot
{
    public static class LocalFilesHandler
    {
        // to do: save it onto local disk or something

        public static List<string> videoNames = new List<string>();

        public static string GetVideoName(int index)
        {
            if (index < videoNames.Count) return videoNames[index];
            else return "";
        }
    }
}