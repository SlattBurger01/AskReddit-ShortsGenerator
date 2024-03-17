using System.Diagnostics;

namespace ProjectCarrot
{
    public static class Main
    {
        public static void Inialize()
        {
            Form1.form.button1.Text = $"Create and upload {Settings.sessionSettingsVideoCount01} videos \r\n using {Settings.sessionsSettings01.Length} settings";

            Form1.form.button2.Text = $"Create {Settings.defaultSettings.video.count} videos";
            Form1.form.button3.Text = $"Upload all videos in folder";
        }

        public static void Button1Click() => CreateAndUploadVideos(Settings.sessionsSettings01);
        public static void Button7Click() => CreateAndUploadVideos(Settings.sessionsSettings02);

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
            
        }

        public static void CreateAndUploadVideos(SessionSettings[] settingsGroup)
        {
            if (!CanContinue(true)) return;

            for (int i = 0; i < settingsGroup.Length; i++)
            {
                CreateVideos(settingsGroup[i]);
                UploadVideos(settingsGroup[i]);
            }
        }

        // ----- ----- ----- ----- -----
        private static void CreateVideos(SessionSettings settings)
        {
            AskRedditSurfer.SetUp(settings.video.rUrl);

            switch (settings.video.type)
            {
                case VideoType.postAndComments:
                    AskRedditSurfer.CreateVideos(settings.video.count, settings);
                    break;
                case VideoType.postAndDescription:
                    TrueOffMyChestReddit.CreateVideos(settings.video.count, settings);
                    break;
            }
        }

        private static void UploadVideos(SessionSettings settings)
        {
            VideoUploader.SetUp(settings.uploadPerson);
            VideoUploader.UploadVideos(settings);
        }

        private static bool second = false;

        private static bool CanContinue(bool create)
        {
            string[] files1 = Directory.GetFiles(LocalPaths.filesPath);
            string[] files2 = Directory.GetFiles(LocalPaths.completedVideosFolder);

            if (second)
            {
                for (int i = 0; i < files1.Length; i++) File.Delete(files1[i]);
                if (create) for (int i = 0; i < files2.Length; i++) File.Delete(files2[i]);

                KillChrome();

                return true;
            }

            bool b1 = CanUpload_CheckFiles(files1);
            bool b2 = !CanUpload_CheckFiles(files2) && create;

            bool chrome = ChromeIsOpened();

            bool pyttsx = Settings.speechType == TextToSpeechType.pyttsx3;

            if (!b1) Form1.form.label2.Text = "Download folder is not empty! Click any button to clear target folder.";
            if (b2) Form1.form.label6.Text = "Created videos folder is not empty! Click any button to clear target folder.";
            if (chrome) Form1.form.label7.Text = "Chrome is running, therefore uploading will fail";
            if (pyttsx) Form1.form.label8.Text = "Pyttsx3 is selected as speech type. (Not an error, just make sure you wanna use it)";

            second = true;
            return b1 && !b2 && !chrome && !pyttsx;
        }

        private static bool CanUpload_CheckFiles(string[] files) { return files.Length == 0; }

        private static bool ChromeIsOpened() => GetChromeProcesses().Length > 0;

        private static void KillChrome()
        {
            Process[] chromeInstances = GetChromeProcesses();

            for (int i = 0; i < chromeInstances.Length; i++) { chromeInstances[i].Kill(); }
        }

        private static Process[] GetChromeProcesses() => Process.GetProcessesByName("chrome");
    }
}