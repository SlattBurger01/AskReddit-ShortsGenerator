using System.Diagnostics;
using System.Windows.Forms;

namespace ProjectCarrot
{
    public static class Main
    {
        public static void Inialize()
        {
            Form1.form.button1.Text = $"Create and upload {Settings.sessionSettingsVideoCount} videos \r\n using {Settings.sessionsSettings.Length} settings";

            Form1.form.button2.Text = $"Create {Settings.defaultSettings.videoCount} videos";
            Form1.form.button3.Text = $"Upload all videos in folder";

            RedditSurfer.onBeforeNewVideoGeneratingStart += VideoEditor.ClearCommentsText;
        }

        /// <summary> Create and upload </summary>
        public static void Button1Click()
        {
            if (!CanContinue(true)) return;

            for (int i = 0; i < Settings.sessionsSettings.Length; i++)
            {
                CreateVideos(Settings.sessionsSettings[i]);
                UploadVideos(Settings.sessionsSettings[i]);
            }
        }

        /// <summary> Create only </summary>
        public static void Button2Click()
        {
            if (!CanContinue(true)) return;

            CreateVideos(Settings.defaultSettings);
        }

        /// <summary> Upload only </summary>
        public static void Button3Click()
        {
            if (!CanContinue(false)) return;

            UploadVideos(Settings.defaultSettings);
        }

        /// <summary> Test </summary>
        public static void Button4Click()
        {
            string text = "At one of my first jobs as a graphic designer, a client called me into his office and pulled up a photo he took. He then asked me if I could turn it around. No, not rotate it, but turn the viewpoint around. He wanted to see what was behind the camera when he originally took the photo�. , God, I have so many stories from that job.  Someone I know argued with me that I do not live anywhere near Canada. He got pretty nasty about it too. , When I showed him proof he said �well I only know Texas�. , I live right outside of Detroit, Michigan. Closer to Canada that I am to any other state. , I have also had a few people argue with me that I do not live in the eastern time zone. I guess they know better than someone who actually lives here.";

            Debug.WriteLine(TextsManager.GetFixedText(text));
        }


        // ----- ----- ----- ----- -----
        private static void CreateVideos(SessionSettings settings)
        {
            RedditSurfer.SetUp(settings.redditUrl);
            RedditSurfer.CreateVideos(settings.videoCount, settings);
        }

        private static void UploadVideos(SessionSettings settings)
        {
            VideoUploader.SetUp(settings.uploadPerson);
            VideoUploader.UploadVideos(settings);
        }

        private static bool second = false;

        private static bool CanContinue(bool create)
        {
            string[] files1 = Directory.GetFiles(Paths.filesPath);
            string[] files2 = Directory.GetFiles(Paths.completedVideosFolder);

            if (second)
            {
                for (int i = 0; i < files1.Length; i++) File.Delete(files1[i]);
                if (create) for (int i = 0; i < files2.Length; i++) File.Delete(files2[i]);

                KillChrome();

                return true;
            }

            bool b1 = CanUpload_CheckFiles(files1);
            bool b2 = CanUpload_CheckFiles(files2) && create;

            bool chrome = ChromeIsOpened();

            if (!b1) Form1.form.label2.Text = "Download folder is not empty! Click any button to clear target folder.";
            if (!b2) Form1.form.label6.Text = "Created videos folder is not empty! Click any button to clear target folder.";
            if (chrome) Form1.form.label7.Text = "Chrome is running, therefore uploading will fail";

            second = true;
            return b1 && b2 && !chrome;
        }

        private static bool CanUpload_CheckFiles(string[] files) => files.Length == 0;

        private static bool ChromeIsOpened() => GetChromeProcesses().Length > 0;

        private static void KillChrome()
        {
            Process[] chromeInstances = GetChromeProcesses();

            for (int i = 0; i < chromeInstances.Length; i++) { chromeInstances[i].Kill(); }
        }

        private static Process[] GetChromeProcesses() => Process.GetProcessesByName("chrome");
    }
}