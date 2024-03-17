/// local & login data

namespace ProjectCarrot
{
    // --- LOCAL DATA
    public static class FileNames
    {
        public static readonly string postName = "postH";
        public static readonly string postAudio = "postA";

        public static readonly string commentName = "commentH";
        public static readonly string commentAudio = "commentA";
    }

    public static class LocalPaths
    {
        private static readonly string projectPath = "E:\\OtherProjects\\ProjectCarrot";

        private static readonly string generatedVideosPath = "E:\\ContentGenerator";

        public static readonly string addBlockPath = "C:\\Users\\kocna\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Extensions\\gighmmpiobklfepjocnamgkkbiglidom\\5.6.0_0.crx"; // new machine

        public static readonly string adAwayAddBlock = "C:\\Users\\kocna\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Extensions\\dgjbaljgolmlcmmklmmeafecikidmjpi\\1.9.2_0.crx";

        public static readonly string pythonPath = "C:\\Users\\kocna\\AppData\\Local\\Programs\\Python\\Python311\\python.exe"; // make sure this path is to actuall python, not just its shortcut

        // this should not be necessary to change
        public static readonly string videoEditorPath = $"{projectPath}\\VideoEditor\\Main.py";
        public static readonly string customReaderPath = $"{projectPath}\\Reader\\Main.py";

        /// <summary> includes "/" </summary>
        public static readonly string filesPath = $"{generatedVideosPath}\\";

        public static readonly string completedVideosFolder = $"{generatedVideosPath}\\CompletedVideos";
        public static readonly string uploadedVideosFolder = $"{generatedVideosPath}\\UploadedVideos";
    }

    public static class LogingData
    {
        public static readonly string userNameReddit = "Slatt_270";
        public static readonly string userNameTtsFree = "Slatt";
        public static readonly string gmailVoiceMaker = "slatt.burger@gmail.com";

        public static readonly string password = "QfdlIOp5468sQ&ss";
    }

    public static class ChromePersons
    {
        public static readonly string person1 = "Person 1";
        public static readonly string person2 = "Person 2";
        public static readonly string person3 = "Person 3";
    }
}