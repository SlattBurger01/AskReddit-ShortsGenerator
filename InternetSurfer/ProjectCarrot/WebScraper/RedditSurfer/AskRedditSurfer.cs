using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProjectCarrot.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Base = ProjectCarrot.WebScraperBase;
using rBase = ProjectCarrot.RedditSurferBase;

namespace ProjectCarrot
{
    public static class AskRedditSurfer
    {
        public static readonly string linePause = ",";

        public static ChromeDriver driver;

        public static void SetUp(string Url)
        {
            driver = rBase.SetupDriverForVideoGeneration();

            driver.Manage().Window.Size = Settings.browserSize;

            TextReader.SetUpReader(Settings.speechType, driver);
            rBase.OpenNewRedditPage(Url);

            rBase.WaitForPostsLoad();

            rBase.TrySwitchToDarkMode();
        }

        public static void OpenReddit() => driver.SwitchTo().Window(driver.WindowHandles.Last());
        public static void OpenReader() => driver.SwitchTo().Window(driver.WindowHandles.First());
        // ----- ----- ----- -----

        public static void CreateVideos(int count, SessionSettings settings, int startPost = 0)
        {
            int vCount = startPost;
            int i = startPost;

            while (vCount < count + startPost)
            {
                bool created = CreateNew(i, vCount, settings);

                if (created) vCount++;

                i++;
            }

            driver.Quit();
        }

        /// <param name="postId"> id of VISIBLE post, therefore the first one is not reflected </param>
        /// <returns> if video was created </returns>
        private static bool CreateNew(int postId, int videoId, SessionSettings settings)
        {
            VideoEditor.ClearCommentsText();

            Debug.WriteLine($"Creating new video ({videoId}), post: {postId}!");

            OpenReddit();

            ReadOnlyCollection<IWebElement> posts = Base.GetElements_X(AskRedditXPaths.posts);

            if (rBase.PostIsPromoted(posts[postId + 1])) return false;

            rBase.OpenPost_(postId + 2, true);

            IWebElement header = Base.WaitForElement(AskRedditXPaths.postHeader);

            LocalFilesHandler.videoNames.Add(header.Text);

            rBase.TakeScreenshot(Base.GetElement_X(AskRedditXPaths.post), FileNames.postName, 0);
            TextReader.ReadText(header.Text, FileNames.postAudio, Settings.speechType, out _);

            ReadAndScreenshotComments(out bool generateSubs);

            while (true)
            {
                if (AudioFilesHandler.AllAudiosAreDownloaded()) break;

                Thread.Sleep(50);
            }

            driver.Navigate().Back();
            AudioFilesHandler.TryRenameAudioFiles();

            VideoEditor.EditVideo(videoId, settings.sessionName, settings, generateSubs);

            return true;
        }

        private static void ReadAndScreenshotComments(out bool generateSubs)
        {
            if (!rBase.WaitForCommentsToLoad(out ReadOnlyCollection<IWebElement> comments, out string cPath))
            {
                Debug.WriteLine($"Comments not found on {cPath}");

                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_1);
                Base.TryClickElement(AskRedditXPaths.notLoadedComments_RetryButton_2);

                rBase.WaitForCommentsToLoad(out comments, out cPath);
            }

            Debug.WriteLine($"Comments found on {cPath} (last: {comments.Last().Text})");

            List<int> selectedComments = GetSuitableComments(comments, cPath, out generateSubs);

            for (int i = 0; i < selectedComments.Count; i++)
            {
                Debug.WriteLine($"sComment {i} = {selectedComments[i]}");

                IWebElement cLocalsc = rBase.GetCommentsTextParent(cPath, selectedComments[i]);
                string t = rBase.GetCommentText(cLocalsc, true);

                Debug.WriteLine(t);
            }

            int currentWordCount = 0;

            Debug.WriteLine($"Comments ({selectedComments.Count})");

            for (int i = 0; i < selectedComments.Count; i++)
            {
                int id = selectedComments[i];

                ReadCommentAndTakeScreenshot(comments[id], id, cPath, i, !generateSubs, out string t);

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

            IWebElement cLocal = rBase.GetCommentsTextParent(cPath, id);

            string t = rBase.GetCommentText(cLocal, true);

            if (takeScreenshot) rBase.TakeScreenshot(selectedComment, $"{FileNames.commentName}-{commentIndex}", 10);
            TextReader.ReadText(t, $"{FileNames.commentAudio}-{commentIndex}", Settings.speechType, out text);
        }

        private static List<int> GetSuitableComments(ReadOnlyCollection<IWebElement> comments, string cPath, out bool singleComment)
        {
            List<int> targetComments = new List<int>();
            List<int> suitableComments = new List<int>();

            int totalWordCount = 0; // if single comment mode triggered: comment char count

            singleComment = false;

            for (int i = 0; i < comments.Count; i++)
            {
                if (!rBase.CommentIsSuitable(cPath, i, out IWebElement? e)) continue;

                string cText = rBase.GetCommentText(rBase.GetCommentsTextParent(cPath, i));

                if (cText.Length == 0) continue; // comment was propably removed by moderator

                int wCount = TextUtils.GetWordCount(cText);

                if (WordCountComment(wCount, cText)) continue;

                if (!singleComment)
                {
                    singleComment = e.Size.Height > Settings.maxCommentHeight || cText.Length > Settings.minCharsForSingleComment;
                }

                if (singleComment && Settings.enableSubs) // longer than something ---> one comment per video is triggered
                {
                    if (cText.Length < totalWordCount) continue;

                    Debug.WriteLine("Single comment per video triggered!");

                    targetComments = new List<int>() { i }; // don't return (some other comment could be longer)
                    totalWordCount = cText.Length;

                    continue;
                }

                if (WordCountAll(suitableComments.Count + 1, totalWordCount + wCount)) break;

                Debug.WriteLine($"Comment {i} is suitable ({wCount})");

                suitableComments.Add(i);
                totalWordCount += wCount;

                if (wCount >= Settings.minCommentWordCount) targetComments.Add(i);
            }

            if (targetComments.Count == 0) targetComments = suitableComments;

            return targetComments;
        }

        /// <returns> if comment has exeeded limit </returns>
        public static bool WordCountComment(int wCount, string cText)
        {
            if (wCount > Settings.maxWordCountPerComment) return true;
            if (wCount < Settings.minWordCountPerComment) return true;
            if (cText.Length > Settings.maxCharsCountPerComment) return true;

            return false;
        }

        /// <returns> if comment has exeeded limit </returns>
        private static bool WordCountAll(int suitableCommsCount, int totalWordCount)
        {
            if (suitableCommsCount > Settings.maxCommentCountPerVideo) return true;
            if (totalWordCount > Settings.maxWordCountPerVideo) return true;

            return false;
        }
    }
}