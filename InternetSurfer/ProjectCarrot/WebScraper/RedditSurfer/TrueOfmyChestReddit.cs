using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProjectCarrot.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Base = ProjectCarrot.WebScraperBase;
using rBase = ProjectCarrot.RedditSurferBase;

namespace ProjectCarrot
{
    /// <summary> To setup class: use AskRedditSurfer.SetUp() </summary>
    public static class TrueOffMyChestReddit
    {
        public static ChromeDriver driver => AskRedditSurfer.driver;

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

        private static bool CreateNew(int postId, int videoId, SessionSettings settings)
        {
            VideoEditor.ClearCommentsText();

            AskRedditSurfer.OpenReddit();

            ReadOnlyCollection<IWebElement> posts = Base.GetElements_X(AskRedditXPaths.posts);

            if (rBase.PostIsPromoted(posts[postId + 1])) return false;

            rBase.OpenPost_(postId + 2, true);

            // ---- ---- ALREADY OPENED ---- ----
            string postText = rBase.GetPostDescription();

            Debug.WriteLine($"postTextCount = {postText.Length}");

            if (AskRedditSurfer.WordCountComment(TextUtils.GetWordCount(postText), postText)) return false;

            IWebElement header = Base.WaitForElement(AskRedditXPaths.postHeader);

            LocalFilesHandler.videoNames.Add(header.Text);

            IWebElement e1 = Base.GetElement_X(AskRedditXPaths.postUserHeader);
            IWebElement e2 = Base.GetElement_X(AskRedditXPaths.postHeaderTextOnly);

            int l = e2.Location.Y - e1.Location.Y;

            Debug.WriteLine($"dist: {l}");

            rBase.TakeScreenshot(e2, FileNames.postName, 22, 47, l + 5, l + 10);
            TextReader.ReadText(header.Text, FileNames.postAudio, Settings.speechType, out _);

            TextReader.ReadText(postText, $"{FileNames.commentName}-{0}", Settings.speechType, out _);
            VideoEditor.AddCommentText(postText);

            while (true)
            {
                if (AudioFilesHandler.AllAudiosAreDownloaded()) break;

                Thread.Sleep(50);
            }

            driver.Navigate().Back();
            AudioFilesHandler.TryRenameAudioFiles();

            VideoEditor.EditVideo(videoId, settings.sessionName, settings, true);

            return true;
        }
    }
}