using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V111.Audits;
using OpenQA.Selenium.DevTools.V113.FedCm;
using ProjectCarrot.Text;
using Selenium.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using Base = ProjectCarrot.WebScraperBase;
using rUtils = ProjectCarrot.RedditSurferUtils;

namespace ProjectCarrot
{
    public static class RedditSurfer
    {
        public static readonly string linePause = ",";

        public static ChromeDriver driver;

        public static void SetUp(string Url)
        {
            SetupDriver();

            driver.Manage().Window.Size = Settings.browserSize;

            SetupReaderPage();
            SetupRedditPage(Url);

            WaitForPostsLoad();

            TrySwitchToDarkMode();
        }

        public static void WaitForPostsLoad()
        {
            float elapsed = 0;

            while (true)
            {
                ReadOnlyCollection<IWebElement> posts = Base.GetElements_X(AskRedditXPaths.posts);

                if (posts.Count > 2) return; // two should be enough for some time (rest should load up while working with these comments)

                Thread.Sleep(Base.waitPause);
                elapsed += Base.waitPause;

                if (elapsed >= Base.maxWaitTime) return;
            }
        }

        private static void SetupDriver()
        {
            bool installAddblock = Settings.speechType == TextToSpeechType.ttsfree;

            ChromeOptions options = Base.GetDefaultOptions();

            // disable "save password" popup
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddArgument("mute-audio");

            if (installAddblock) options.AddExtension(LocalPaths.adAwayAddBlock);

            driver = new ChromeDriver(options);
            Base.driver = driver;

            if (installAddblock)
            {
                while (true)
                {
                    // Close addblock instalation tab
                    if (driver.WindowHandles.Count > 1)
                    {
                        Debug.WriteLine("Closing window");

                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.First());
                        break;
                    }
                }
            }
        }

        public static void CreateVideos(int count, SessionSettings settings, int startPost = 0)
        {
            DateTime startTime = DateTime.Now;

            int vCount = startPost;
            int i = startPost;

            while (vCount < count + startPost)
            {
                bool created = CreateNew(i, vCount, settings);

                if (created) vCount++;

                i++;
            }

            driver.Quit();

            TimeSpan fTime = DateTime.Now - startTime;

            Debug.WriteLine($"{count} videos created in {fTime}");

            Form1.form.label3.Text = $"{count} videos created in {fTime.ToString(@"hh\:mm\:ss")}";
        }

        public static Action onBeforeNewVideoGeneratingStart = delegate { };

        /// <param name="postId"> id of VISIBLE post, therefore the first one is not reflected </param>
        /// <returns> if video was created </returns>
        private static bool CreateNew(int postId, int videoId, SessionSettings settings)
        {
            onBeforeNewVideoGeneratingStart.Invoke();

            Debug.WriteLine($"Creating new video ({videoId}), post: {postId}!");

            OpenReddit();

            ReadOnlyCollection<IWebElement> posts = Base.GetElements_X(AskRedditXPaths.posts);

            //if (posts[postId +1].GetAttribute("style") == "") -- its something like height = 10px and happens when promoted content is not loaded

            if (rUtils.PostIsPromoted(posts[postId + 1])) return false;

            try { OpenPost_(postId + 2); }
            catch (Exception e)
            {
                IWebElement element = Base.GetElement_X(rUtils.GetPostPath(postId + 2));

                if (element.Size.Height == 0) return false; // fcked up comment 

                Debug.WriteLine($"Post could not be opened! {e.Message}");

                Base.ClickElement(AskRedditXPaths.interestsTab);
                Thread.Sleep(100);
                OpenPost_(postId + 2);
            }

            IWebElement header = Base.WaitForElement(AskRedditXPaths.postHeader);

            LocalFilesHandler.videoNames.Add(header.Text);

            rUtils.TakeScreenshot(Base.GetElement_X(AskRedditXPaths.post), FileNames.postName, 0);
            TextReader.ReadText(header.Text, FileNames.postAudio, Settings.speechType, out _);

            //Thread.Sleep(2000);

            ReadAndScreenshotComments();

            while (true)
            {
                if (AudioFilesHandler.AllAudiosAreDownloaded()) break;

                Thread.Sleep(50);
            }

            driver.Navigate().Back();
            AudioFilesHandler.TryRenameAudioFiles();

            VideoEditor.EditVideo(videoId, settings.sessionName, settings);

            return true;
        }

        private static void ReadAndScreenshotComments()
        {
            Thread.Sleep(1000);

            if (!rUtils.WaitForCommentsToLoad(out ReadOnlyCollection<IWebElement> comments, out string cPath))
            {
                Debug.WriteLine($"Comments not found on {cPath}");

                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_1);
                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_2);

                rUtils.WaitForCommentsToLoad(out comments, out cPath);
            }

            Debug.WriteLine($"Comments found on {cPath} (last: {comments.Last().Text})");

            List<int> selectedComments = GetSuitableComments(comments, cPath);

            for (int i = 0; i < selectedComments.Count; i++)
            {
                Debug.WriteLine($"sComment {i} = {selectedComments[i]}");

                IWebElement cLocalsc = rUtils.GetCommentsTextParent(cPath, selectedComments[i]);
                string t = rUtils.GetCommentText(cLocalsc, true);

                Debug.WriteLine(t);
            }

            int currentWordCount = 0;

            bool takeSrc = selectedComments.Count > 1;

            Debug.WriteLine($"Comments ({selectedComments.Count})");

            for (int i = 0; i < selectedComments.Count; i++)
            {
                int id = selectedComments[i];

                ReadCommentAndTakeScreenshot(comments[id], id, cPath, i, takeSrc, out string t);

                currentWordCount += TextUtils.GetWordCount(t);

                VideoEditor.AddCommentText(t);

                Debug.WriteLine($"comment: {i}, {t}");
                Debug.WriteLine($"comment: {TextUtils.GetWordCount(t)}, {currentWordCount}");
                Debug.WriteLine($"________________________________");

                if (currentWordCount >= 100) break;
            }
        }

        private static void ReadCommentAndTakeScreenshot(IWebElement selectedComment, int id, string cPath, int commentIndex, bool takeScreenshot, out string text)
        {
            Base.ScrollToElement(selectedComment);

            IWebElement cLocal = rUtils.GetCommentsTextParent(cPath, id);

            string t = rUtils.GetCommentText(cLocal, true);

            if (takeScreenshot) rUtils.TakeScreenshot(selectedComment, $"{FileNames.commentName}-{commentIndex}", 10);
            TextReader.ReadText(t, $"{FileNames.commentAudio}-{commentIndex}", Settings.speechType, out text);
        }

        private static List<int> GetSuitableComments(ReadOnlyCollection<IWebElement> comments, string cPath)
        {
            List<int> targetComments = new List<int>();
            List<int> suitableComments = new List<int>();

            int totalWordCount = 0; // if single comment mode triggered: comment char count

            bool singleComment_ = false;

            for (int i = 0; i < comments.Count; i++)
            {
                if (!rUtils.CommentIsSuitable(cPath, i, out IWebElement? e)) continue;

                string cText = rUtils.GetCommentText(rUtils.GetCommentsTextParent(cPath, i));

                if (cText.Length == 0) continue; // comment was propably removed by moderator

                int wCount = TextUtils.GetWordCount(cText);

                if (Settings_1(wCount, cText)) continue;

                if (!singleComment_)
                {
                    singleComment_ = e.Size.Height > Settings.maxCommentHeight || cText.Length > Settings.minCharsForSingleComment;
                }

                if (singleComment_) // longer than something ---> one comment per video is triggered
                {
                    if (cText.Length < totalWordCount) continue;

                    Debug.WriteLine("Single comment per video triggered!");

                    targetComments = new List<int>() { i }; // don't return (some other comment could be longer)
                    totalWordCount = cText.Length;

                    continue;
                }

                if (Settings_2(suitableComments.Count + 1, totalWordCount + wCount)) break;

                Debug.WriteLine($"Comment {i} is suitable ({wCount})");

                suitableComments.Add(i);
                totalWordCount += wCount;

                if (wCount >= Settings.targetCommentMinWordCount) targetComments.Add(i);
            }

            if (targetComments.Count == 0) targetComments = suitableComments;

            return targetComments;
        }

        // to do: rename this function 
        private static bool Settings_1(int wCount, string cText)
        {
            if (wCount > Settings.maxWordCountPerComment) return true;
            if (wCount < Settings.minWordCountPerComment) return true;
            if (cText.Length > Settings.maxCharsCountPerComment) return true;

            return false;
        }

        // to do: rename this function
        private static bool Settings_2(int suitableCommsCount, int totalWordCount)
        {
            if (suitableCommsCount > Settings.maxCommentCountPerVideo) return true;
            if (totalWordCount > Settings.maxWordCountPerVideo) return true;

            return false;
        }

        private static void SetupReaderPage() => TextReader.SetUpReader(Settings.speechType, driver);

        private static void SetupRedditPage(string url)
        {
            Debug.WriteLine("Setting up reddit");

            Base.OpenUrlInNewWindow(url);

            Base.TryClickElement(AskRedditXPaths.acceptButton);

            if (Settings.loginToReddit) rUtils.LoginToReddit();
        }

        private static void OpenPost_(int sPost)
        {
            string path = $"{rUtils.GetPostPath(sPost)}/{AskRedditXPaths.openPostButton_local}";

            Debug.WriteLine($"Opening post ({sPost}) at path ({path})");

            Base.ClickElement(path);

            Base.ScrollToTop(); // you have to reset position immidiatelly to make correct screenshots
        }

        // (it switches usually automatically), but this should worked on ( switch only if is in light mode )
        private static void TrySwitchToDarkMode()
        {
            IWebElement colorBackground = Base.GetElement_X("/html/body/div[1]/div");
            if (!colorBackground.GetAttribute("style").Contains("#FFFFFF")) return;

            IWebElement settings = Base.GetElement_X(AskRedditXPaths.settings);
            settings.Click();

            Base.ClickElement(AskRedditXPaths.darkMode);

            settings.Click();
        }

        public static void OpenReddit() => driver.SwitchTo().Window(driver.WindowHandles.Last());
        public static void OpenReader() => driver.SwitchTo().Window(driver.WindowHandles.First());
    }
}