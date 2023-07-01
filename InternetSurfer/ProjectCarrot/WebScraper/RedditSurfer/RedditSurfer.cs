using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V111.Audits;
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

            //Thread.Sleep(2000); // wait for reddit to load enough posts

            /*while (true)
            {
                ReadOnlyCollection<IWebElement> posts = Base.GetElements_X(AskRedditXPaths.posts);

                if (posts.Count > 2) break; // two should be enough for some time (rest should load up while working with these comments)

                Thread.Sleep(50);
            }*/

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

            if (installAddblock) options.AddExtension(Paths.addBlockPath);

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

            /*ReadOnlyCollection<IWebElement> comments = rUtils.GetComments(out string cPath);

            while (comments.Count == 0)
            {
                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_1);
                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_2);
                Thread.Sleep(100);

                comments = rUtils.GetComments(out cPath);
            }*/

            Debug.WriteLine($"Last: {comments.Last().Text}, path = {cPath}");

            List<int> selectedComments = GetSuitableComments(comments, cPath);

            for (int i = 0; i < selectedComments.Count; i++)
            {
                Debug.WriteLine($"sComment {i} = {selectedComments[i]}");

                IWebElement cLocalsc = rUtils.GetCommentsTextParent(cPath, selectedComments[i]);
                string t = rUtils.GetCommentText(cLocalsc, true);

                Debug.WriteLine(t);
            }

            int currentWordCount = 0;

            Debug.WriteLine($"Comments ({selectedComments.Count})");

            for (int i = 0; i < selectedComments.Count; i++)
            {
                int id = selectedComments[i];

                ReadCommentAndTakeScreenshot(comments[id], id, cPath, i, out string t);

                currentWordCount += TextUtils.GetWordCount(t);

                VideoEditor.commentsText += t;

                Debug.WriteLine($"comment: {i}, {t}");
                Debug.WriteLine($"comment: {TextUtils.GetWordCount(t)}, {currentWordCount}");
                Debug.WriteLine($"________________________________");

                if (currentWordCount >= 100) break;
            }
        }

        private static void ReadCommentAndTakeScreenshot(IWebElement selectedComment, int id, string cPath, int commentIndex, out string text)
        {
            Base.ScrollToElement(selectedComment);

            IWebElement cLocal = rUtils.GetCommentsTextParent(cPath, id);

            string t = rUtils.GetCommentText(cLocal, true);

            rUtils.TakeScreenshot(selectedComment, $"{FileNames.commentName}-{commentIndex}", 10);
            TextReader.ReadText(t, $"{FileNames.commentAudio}-{commentIndex}", Settings.speechType, out text);
            //Thread.Sleep(500);
        }

        private static List<int> GetSuitableComments(ReadOnlyCollection<IWebElement> comments, string cPath)
        {
            List<int> targetComments = new List<int>();
            List<int> suitableComments = new List<int>();

            int totalWordCount = 0;

            for (int i = 0; i < comments.Count; i++)
            {
                if (!rUtils.CommentIsSuitable(cPath, i)) continue;

                //IWebElement textParent = GetCommentsTextParent(cPath, i);
                string cText = rUtils.GetCommentText(rUtils.GetCommentsTextParent(cPath, i));

                if (cText.Length == 0) continue; // comment was propably removed by moderator

                int wCount = TextUtils.GetWordCount(cText);

                if (wCount > Settings.maxWordCountPerComment) continue;
                if (wCount < Settings.minWordCountPerComment) continue;
                if (cText.Length > Settings.maxCharsCountPerComment) continue;

                if (suitableComments.Count + 1 > Settings.maxCommentCountPerVideo) break;
                if (totalWordCount + wCount > Settings.maxWordCountPerVideo) break;

                Debug.WriteLine($"Comment {i} is suitable ({wCount})");

                suitableComments.Add(i);
                totalWordCount += wCount;

                if (wCount >= Settings.targetCommentMinWordCount) targetComments.Add(i);
            }

            if (targetComments.Count == 0) targetComments = suitableComments;

            return targetComments;
        }

        private static void SetupReaderPage() => TextReader.SetUpReader(Settings.speechType, driver);

        private static void SetupRedditPage(string Url)
        {
            Debug.WriteLine("Setting up reddit");

            driver.SwitchTo().NewWindow(WindowType.Tab);

            driver.Navigate().GoToUrl(Url);

            Base.TryClickElement(AskRedditXPaths.acceptButton);

            if (Settings.loginToReddit) LoginToReddit();
        }

        private static void LoginToReddit()
        {
            Base.ClickElement(AskRedditXPaths.loginButton); 

            Base.WaitForElement(AskRedditXPaths.loginIFrame);

            driver.SwitchTo().Frame(0);

            Base.SendKeysToElement(AskRedditXPaths.usernameInput, LogingData.userNameReddit);
            Base.SendKeysToElement(AskRedditXPaths.passwordInput, LogingData.password);

            Base.ClickElement(AskRedditXPaths.finalLoginButton);
        }

        private static void OpenPost_(int sPost)
        {
            //string path = AskRedditXPaths.posts + $"[{sPost}]/{AskRedditXPaths.openPostButton_local}";
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

        public static void Test()
        {
            string v = "That kids reaction broke my heart. Whether it was a \"joke\" or not, he was confused and freaked out but it was an authority figure so he was going to do it, even though all his instincts are telling him to gtfo, he was still gonna do it. Its fucking chilling seeing how quickly someone can go from regular kid to abuse victim.\r\n\r\nETA For the people asking, this is the one that was posted here that I saw. For everyone else I'm sorry.\r\n\r\nhttps://www.reddit.com/r/interestingasfuck/comments/12iekyu/a_weird_video_of_the_dalai_lama_asking_an_indian/?utm_source=share&utm_medium=ios_app&utm_name=iossmf";

            Debug.WriteLine(TextUtils.RemoveHyperlinks(v));
        }
    }
}