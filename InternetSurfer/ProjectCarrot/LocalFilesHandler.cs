using System.Diagnostics;

namespace ProjectCarrot
{
    public static class LocalFilesHandler
    {
        public static List<string> videoNames = new List<string>();

        public static string GetVideoName(int index)
        {
            if (index < videoNames.Count) return videoNames[index];
            else return "";
        }

        /// <summary> Moves 'files' to folder and adds 'customIndex' before their type </summary>
        public static void MoveFilesToFolder(string[] files, DirectoryInfo targetFolder, string customIndex = "")
        {
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo info = new FileInfo(files[i]);

                string fileName = RemoveType(info, out string type);

                string newPath = @$"{targetFolder.FullName}\{fileName}{customIndex}{type}";

                Debug.WriteLine($"Moving file {files[i]} to {newPath}");

                Directory.Move(files[i], newPath);
            }
        }

        /// <returns> Files name without it's type </returns>
        private static string RemoveType(FileInfo info, out string type)
        {
            type = info.Extension;

            string name = info.Name;

            return name.Remove(name.Length - type.Length);
        }

        public static void DeleteFiles(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                Debug.WriteLine($"Deleting file {files[i]}");

                File.Delete(files[i]);
            }
        }
    }
}