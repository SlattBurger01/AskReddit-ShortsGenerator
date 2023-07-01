using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCarrot
{
    public static class AudioFilesHandler
    {
        private static List<string> targetNames = new List<string>();

        /// <summary> Adds name into 'targetNames' array </summary>
        public static void AddTargetName(string name) => targetNames.Add(name);

        /// <summary> Renames downloaded audio files on drive based on 'targetNames' and creation time & clears targetNames </summary>
        public static void TryRenameAudioFiles()
        {
            Debug.WriteLine("Renaming audiofiles:");
            for (int i = 0; i < targetNames.Count; i++) Debug.WriteLine($"target name {i}: {targetNames[i]}");

            VideoEditor.CreateSpecialFolder();

            string[] files = Directory.GetFiles(Paths.filesPath);

            int cNameIndex = 0;

            // --- Get files
            List<FileInfo> audioFiles = new List<FileInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(files[i]);

                if (fileInfo.Name.StartsWith("ttsMP3.com_") || fileInfo.Name.StartsWith("mp3-output-ttsfree"))
                {
                    audioFiles.Add(fileInfo);
                }
            }
            // ---

            // --- Sorts files based on it's creation time
            int n = audioFiles.Count;
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    if (audioFiles[i - 1].CreationTime > audioFiles[i].CreationTime)
                    {
                        (audioFiles[i], audioFiles[i - 1]) = (audioFiles[i - 1], audioFiles[i]);
                        swapped = true;
                    }
                }
                n--;
            } while (swapped);
            // ---

            for (int i = 0; i < audioFiles.Count; i++)
            {
                FileInfo info = audioFiles[i];

                RenameAudioFile(info.FullName, info, ref cNameIndex);
            }

            targetNames.Clear();
        }

        private static void RenameAudioFile(string file, FileInfo fileInfo, ref int nameId)
        {
            string newPath = file.Remove(file.Length - fileInfo.Name.Length);
            newPath += targetNames[nameId] + ".mp3";
            nameId++;

            Debug.WriteLine($"New: {newPath}, previous: {file}");

            fileInfo.MoveTo(newPath);
        }

        /// <returns> If all files in download folder not is being downloaded </returns>
        public static bool AllAudiosAreDownloaded()
        {
            string[] files = Directory.GetFiles(Paths.filesPath);

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) == ".crdownload") return false;
            }

            return true;
        }
    }
}
