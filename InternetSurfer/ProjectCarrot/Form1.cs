using OpenQA.Selenium;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProjectCarrot
{
    public partial class Form1 : Form
    {
        public static Form1 form;

        public Form1()
        {
            InitializeComponent();
            form = this;

            form.button1.Text = $"Create and upload {Settings.sessionSettingsVideoCount} videos \r\n using {Settings.sessionsSettings.Length} settings";

            form.button2.Text = $"Create {Settings.defaultSettings.videoCount} videos";
            form.button3.Text = $"Upload all videos in folder";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CanContinue(true)) return;

            for (int i = 0; i < Settings.sessionsSettings.Length; i++)
            {
                CreateVideos(Settings.sessionsSettings[i]);
                UploadVideos(Settings.sessionsSettings[i]);
            }

        }

        // Only video create
        private void button2_Click(object sender, EventArgs e)
        {
            if (!CanContinue(true)) return;

            CreateVideos(Settings.defaultSettings);
        }

        // Only video upload
        private void button3_Click(object sender, EventArgs e)
        {
            if (!CanContinue(false)) return;

            UploadVideos(Settings.defaultSettings);
        }

        // Test
        private void button4_Click(object sender, EventArgs e)
        {

        }

        // ---
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

                return true;
            }

            bool b1 = CanUpload_CheckFiles(files1);
            bool b2 = CanUpload_CheckFiles(files2) && create;

            if (b1) form.label2.Text = "Download folder is not empty! Click any button to clear target folder.";
            if (b2) form.label6.Text = "Created videos folder is not empty! Click any button to clear target folder.";

            second = true;
            return !b1 && !b2;
        }

        private static bool CanUpload_CheckFiles(string[] files)
        {
            if (files.Length == 0) return false;

            return true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}