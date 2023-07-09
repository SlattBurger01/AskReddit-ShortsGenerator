using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Selenium.Extensions;
using Selenium.WebDriver.UndetectedChromeDriver;
using System.Diagnostics;
using Base = ProjectCarrot.WebScraperBase;
using Keys = OpenQA.Selenium.Keys;

namespace ProjectCarrot
{
    public static class VideoUploader
    {
        public static string[] GetVideos() => Directory.GetFiles(LocalPaths.completedVideosFolder);

        private static SlDriver driver;

        public static void SetUp(string person)
        {
            ChromeOptions options = Base.GetDefaultOptions();
            driver = UndetectedChromeDriver.Instance(person, options);
            Base.driver = driver;
        }

        public static void UploadVideos(SessionSettings settings)
        {
            DateTime startTime = DateTime.Now;

            string[] videos = GetVideos();

            if (settings.uploadPlatforms.includeYoutube) UploadVideosToYoutube(videos);
            if (settings.uploadPlatforms.includeTikTok) UploadVideosToTiktok(videos);
            if (settings.uploadPlatforms.includeInstagram) UploadVideosToInstagram(videos);

            MoveVideoFilesToUploaded();

            driver.Quit();

            TimeSpan fTime = DateTime.Now - startTime;

            Debug.WriteLine($"final time (uploading) = {fTime}");

            Form1.form.label4.Text = $"{videos.Length} videos uploaded in {fTime.ToString(@"hh\:mm\:ss")}";
        }

        public static void UploadVideosToYoutube(string[] videos)
        {
            driver.Navigate().GoToUrl("https://www.youtube.com/");

            Base.WaitAndClickElement(YouTubeXPaths.uploadButton);

            Base.WaitAndClickElement(YouTubeXPaths.uploadVideo);

            for (int i = 0; i < videos.Length; i++)
            {
                UploadVideoToYoutube(videos[i], i);
                Thread.Sleep(1000);
            }
        }

        private static void UploadVideoToYoutube(string videoName, int y)
        {
            Base.SendKeysToElement(YouTubeXPaths.videoInput, videoName);

            Base.WaitAndClickElement(YouTubeXPaths.notMadeForKidsButton);

            IWebElement nameInput = Base.GetElement_X(YouTubeXPaths.videoNameInput);

            while (nameInput.Text != string.Empty)
            {
                Debug.WriteLine(nameInput.Text);
                nameInput.SendKeys(Keys.Backspace);
            }

            Debug.WriteLine(nameInput.Text);

            string header = LocalFilesHandler.GetVideoName(y);
            string headerF = header.Length > 100 ? "Thoughts?" : header;

            string[] ts = VideoData.youtubeTags;
            for (int i = 0; i < ts.Length; i++)
            {
                if ((ts[i] + headerF).Length + 2 > 100) break; // +2 (hastag and space)

                headerF += $" #{ts[i]}";
            }

            nameInput.SendKeys(headerF);

            Base.ClickElement(YouTubeXPaths.visibilityButton); // click on visibility

            Base.WaitAndClickElement(YouTubeXPaths.schedulePage);
            Base.WaitAndClickElement(YouTubeXPaths.datePickerButton);

            Thread.Sleep(1000);

            (string, string) tDate = GetNextDate(y);

            IWebElement dateInput = Base.GetElement_X(YouTubeXPaths.dateInput);
            dateInput.Clear();
            dateInput.SendKeys(tDate.Item1);
            dateInput.SendKeys(Keys.Enter);

            Thread.Sleep(1000);

            IWebElement timeInput = Base.GetElement_X(YouTubeXPaths.timeInput);
            timeInput.Clear();
            timeInput.SendKeys(tDate.Item2);

            while (true)
            {
                try { Base.ClickElement(YouTubeXPaths.scheduleButton); }
                catch { break; } // if button was already clicked: it will interrupt click / page was loaded and the button does not exist anymore

                Thread.Sleep(100);
            }

            Base.WaitForElement("/html/body/ytcp-uploads-still-processing-dialog/ytcp-dialog/tp-yt-paper-dialog/div[1]/div/h1");

            while (true)
            {
                if (!Base.ElementExists("/html/body/ytcp-uploads-still-processing-dialog/ytcp-dialog/tp-yt-paper-dialog/div[2]/div/ytcp-video-upload-progress/span", out var e)) break;

                if (!e.Text.Contains("Uploading")) break;

                Thread.Sleep(50);
            }

            driver.Navigate().Refresh();

            Base.TryAcceptAlert();
        }

        private static int schedulesPause = Settings.schedulePause * 15;

        private static (string, string) GetNextDate(int i)
        {
            DateTime currentTime = DateTime.Now;

            double roundedMinutes = Math.Ceiling((float)currentTime.Minute / 15) * 15;

            DateTime t1 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0);
            DateTime finalTime = t1.AddMinutes(roundedMinutes + 30 + schedulesPause * i);

            string date = finalTime.ToString("d MMM yyyy");
            string time = finalTime.ToString("HH:mm");

            return (date, time);
        }

        public static void UploadVideosToTiktok(string[] videos)
        {
            NewTab("https://www.tiktok.com/upload");

            //Thread.Sleep(5000);

            for (int i = 0; i < videos.Length; i++)
            {
                driver.SwitchTo().Frame(0);

                UploadVideoToTiktok(videos[i], i);
            }
        }

        private static void UploadVideoToTiktok(string videoName, int y)
        {
            Base.WaitAndSendKeysToElement(TiktokPaths.videoInput, videoName); // if you're getting null refferece errors try to increase max wait time

            // wait until video is uploaded
            Base.WaitForElement("/html/body/div[1]/div/div/div/div[2]/div[2]/div[2]/div[1]/div/div[1]/div[2]/div", Base.maxUploadInterWaitTime); // some element that is only visible after video is uploaded

            Debug.WriteLine("Video is uploaded !");

            IWebElement descriptionInput = Base.GetElement_X(TiktokPaths.videoDescriptionInput);

            Debug.WriteLine($"input: {descriptionInput}");

            descriptionInput.Click();

            Actions action = new Actions(driver);

            string vName = Base.GetElement_X("/html/body/div[1]/div/div/div/div[2]/div[2]/div[2]/div[1]/div/div[1]/div[2]/div/div/div/div/div/div/div/div/div/span/span").Text; // video input text

            for (int i = 0; i < vName.Length + 2; i++)
            {
                action.SendKeys(descriptionInput, Keys.Backspace);
                Thread.Sleep(50);
            }
            action.Perform();

            string k = LocalFilesHandler.GetVideoName(y);
            if (!string.IsNullOrEmpty(k)) action.SendKeys(descriptionInput, $"{k} ").Perform();

            string[] tags = VideoData.tiktokTags;
            for (int i = 0; i < tags.Length; i++)
            {
                action.SendKeys(descriptionInput, $"#{tags[i]}").Perform();

                Base.WaitForElement(TiktokPaths.hashtagRecomendation); // '#' recomendations
                Thread.Sleep(200);

                action.SendKeys(Keys.Enter).Perform();
                Thread.Sleep(500);
            }

            Base.ClickElement(TiktokPaths.uploadButton); // click post

            //Thread.Sleep(1000);

            driver.Navigate().Refresh();

            Base.TryAcceptAlert();
        }

        public static void UploadVideosToInstagram(string[] videos)
        {
            NewTab("https://www.instagram.com/");

            Thread.Sleep(500);

            for (int i = 0; i < videos.Length; i++)
            {
                UploadVideoToInstagram(videos[i]);
            }
        }

        private static void UploadVideoToInstagram(string video)
        {
            Base.ClickElement(InstagramPaths.uploadButton); // upload button

            Thread.Sleep(1000);

            Base.SendKeysToElement(InstagramPaths.fileInput, video);

            Thread.Sleep(2500);

            Base.ClickElement(InstagramPaths.videoRatioButton); // ratio

            Thread.Sleep(1000);

            Base.ClickElement(InstagramPaths.originaRationButton); // original

            Base.ClickElement("/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/div/div/div[3]/div/div");

            Thread.Sleep(1000);

            Base.ClickElement("/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/div/div/div[3]/div/div");

            Thread.Sleep(1000);

            IWebElement caption = Base.GetElement_X("/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[2]/div/div/div/div[2]/div[1]/div");
            caption.Clear();
            caption.SendKeys(VideoData.tiktokTagsInString);

            Base.ClickElement("/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/div/div/div[3]/div/div"); // share

            while (true)
            {
                Base.TryClickElement(InstagramPaths.tryAgainButton);

                if (Base.ElementExists("/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[1]/div/div[2]/div/span")) break;

                Thread.Sleep(50);
            }

            driver.Navigate().Refresh();
        }

        private static void NewTab(string url)
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl(url);
        }

        private static void MoveVideoFilesToUploaded()
        {
            DirectoryInfo tFolder = Directory.CreateDirectory(@$"{LocalPaths.uploadedVideosFolder}\{VideoEditor.SpecialFolder}");

            string[] files = GetVideos();

            if (Settings.deleteVideosAfterUploaded) LocalFilesHandler.DeleteFiles(files);
            else LocalFilesHandler.MoveFilesToFolder(files, tFolder);
        }

        // --- --- --- --- --- || --- --- --- --- --- || --- --- --- --- --- \\
        public static void Test()
        {
            for (int i = 0; i < 100; i++)
            {
                (string, string) date = GetNextDate(i);

                Debug.WriteLine($"{date.Item1} {date.Item2}");
            }
        }
    }

    public static class VideoData
    {
        public static readonly string[] youtubeTags = new string[] { "shorts", "askreddit", "reddit" };
        public static readonly string[] tiktokTags = new string[] { "fyp", "askreddit", "reddit" };

        public static readonly string youtubeTagsInString = ConnectTags(youtubeTags);
        public static readonly string tiktokTagsInString = ConnectTags(youtubeTags);

        private static string ConnectTags(string[] tags)
        {
            string s = "";

            for (int i = 0; i < tags.Length; i++)
            {
                s += $"#{tags[i]} ";
            }

            return s;
        }
    }
}