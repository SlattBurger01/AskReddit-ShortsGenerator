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
        private static readonly string usedFolder = $@"{LocalPaths.filesPath}\Used\{specialFolder}";

        private static string CreateSpecialFolderName()
        {
            string n = "";

            Random r = new Random();

            for (int i = 0; i < 20; i++)
            {
                n += chars[r.Next(0, chars.Length)];
            }

            Form1.form.label5.Text = $"SpecialFolder = {n}";

            return n;
        }

        public static void CreateSpecialFolder()
        {
            Directory.CreateDirectory($@"{LocalPaths.filesPath}Used\{specialFolder}");

            Debug.WriteLine($"Special folder created ({specialFolder})");
        }

        public static void ClearCommentsText() { commentsText = ""; }

        private static string commentsText = "";

        /// <summary> Adds text 't' into 'commentsText', fixes text, trims end and adds dot if necessary </summary>
        public static void AddCommentText(string t)
        {
            commentsText += $"{TextsManager.GetFixedText(t)} ";

            commentsText = commentsText.TrimEnd();

            if (commentsText.Last() != '.') commentsText += ". ";
        }

        /// <summary> Edits video based on downloaded files and moves / deletes them </summary>
        public static void EditVideo(int videoIdentifier, string sessionName, SessionSettings settings, bool generateSubs)
        {
            string result = "";

            if (Settings.renderVideo) result = CallVideoEditorPythonFile($"{sessionName}_{videoIdentifier} {generateSubs} \"{commentsText}\"");

            Debug.WriteLine(result);

            MoveUsedResources(videoIdentifier, settings);
        }

        private static string CallVideoEditorPythonFile(string args) => PythonHelper.CallPythonFile(LocalPaths.videoEditorPath, args);

        private static void MoveUsedResources(int videoIdentifier, SessionSettings settings)
        {
            string[] files = Directory.GetFiles(LocalPaths.filesPath);

            if (Settings.deleteFilesAfterUsed) LocalFilesHandler.DeleteFiles(files);
            else LocalFilesHandler.MoveFilesToFolder(files, new DirectoryInfo(usedFolder), $"_{videoIdentifier}_{settings.sessionName}");
        }
    }
}