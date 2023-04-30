using System.Diagnostics;

namespace ProjectCarrot
{
    public static class VideoEditor
    {
        private static readonly string alphabbet = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string nums = "0123456789";

        private static readonly string chars = alphabbet + nums;

        public static string SpecialFolder => specialFolder;

        private static readonly string specialFolder = CreateSpecialFolderName();
        private static readonly string usedFolder = $@"E:\HONZA\ContentGenerator\Used\{specialFolder}";

        private static string CreateSpecialFolderName()
        {
            string n = "";

            Random r = new Random();

            for (int i = 0; i < 15; i++)
            {
                n += chars[r.Next(0, chars.Length)];
            }

            Form1.form.label5.Text = $"SpecialFolder = {n}";

            return n;
        }

        public static void CreateSpecialFolder()
        {
            Directory.CreateDirectory($@"E:\HONZA\ContentGenerator\Used\{specialFolder}");

            Debug.WriteLine($"Special folder created ({specialFolder})");
        }

        public static void EditVideo(int videoIdentifier, string sessionName, SessionSettings settings)
        {
            string result = "";

            if (Settings.renderVideo) result = CallPythonFile(Paths.videoEditorPath, $"{sessionName}_{videoIdentifier}");

            Debug.WriteLine(result);

            MoveUsedResources(videoIdentifier, settings);
        }

        private static string CallPythonFile(string file, string args) => PythonHelper.CallPythonFile(file, args);

        private static void MoveUsedResources(int videoIdentifier, SessionSettings settings)
        {
            var s = Directory.GetFiles(@"E:\HONZA\ContentGenerator\");

            for (int i = 0; i < s.Length; i++)
            {
                Debug.WriteLine(s[i]);
            }

            VideoUploader.MoveFilesToFolder(s, new DirectoryInfo(usedFolder), $"_{videoIdentifier}_{settings.sessionName}");
        }

        public static void Test()
        {

        }
    }
}