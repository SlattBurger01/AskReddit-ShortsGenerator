using OpenQA.Selenium;
using ProjectCarrot.Text;
using Selenium.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using Base = ProjectCarrot.WebScraperBase;

namespace ProjectCarrot
{
    public static class RedditSurferUtils
    {
        // currently is not being used
        /*private static string GetCommentUserName(string cPath, int id)
        {
            try { return Base.GetElement_X($"{CommentPath(cPath, id)}/{AskRedditXPaths.commentUserName_1}").Text; }
            catch
            {
                try
                {
                    return Base.GetElement_X($"{CommentPath(cPath, id)}/{AskRedditXPaths.commentUserName_2}").Text;
                }
                catch { return ""; } // element is not a comment but button to open more comments
            }
        }*/

        /// <returns> if comment was posted by moderator </returns>
        public static bool IsModeratorComment(string cPath, int id)
        {
            IWebElement badge;

            try { badge = GetElementOnCommentLocalPath(cPath, id, AskRedditXPaths.badgeLocal_3); } // let it throw error if element was not found
            catch { badge = TryGetElementOnCommentLocalPath(cPath, id, AskRedditXPaths.badgeLocal_4); } // just try to get element at target path

            if (badge == null) return false;

            Debug.WriteLine(badge.Text);

            if (badge.Text == "MOD")
            {
                Debug.WriteLine($"Skipping comment, because it was written by bot");
                return true;
            }

            return false;
        }

        /// <param name="cPath"> used path </param>
        /// <returns> found comments </returns>
        public static ReadOnlyCollection<IWebElement> GetComments(out string cPath)
        {
            ReadOnlyCollection<IWebElement> comments = Base.GetElements_X(cPath = AskRedditXPaths.comments_4);

            if (comments.Count == 0)
            {
                //cPath = AskRedditXPaths.comments_5;
                comments = Base.GetElements_X(cPath = AskRedditXPaths.comments_5);

                if (comments.Count == 0)
                {
                    //cPath = AskRedditXPaths.comments_6;
                    comments = Base.GetElements_X(cPath = AskRedditXPaths.comments_6);
                }
            }

            return comments;
        }

        /// <returns> If comments were loaded (25) or waited more than 4 secs, but found at least one (true), otherwise if comments were not (at least 1) loaded in 1 second or waited longer than 'Base' allows</returns>
        public static bool WaitForCommentsToLoad(out ReadOnlyCollection<IWebElement> comments, out string cPath)
        {
            float elapsedTime = 0;

            while (true)
            {
                comments = GetComments(out cPath);

                //Debug.WriteLine($"{comments.Count()} ({cPath})");

                if (elapsedTime >= 4000 && comments.Count < 25) return comments.Count >= 1;
                if (elapsedTime >= 1000 && comments.Count < 1) return false;

                if (comments.Count >= 25) return true;

                Thread.Sleep(Base.waitPause);
                elapsedTime += 50;

                if (elapsedTime > Base.maxWaitTime) return false;
            }
        }

        public static IWebElement GetCommentsTextParent(string cPath, int i) => GetElementOnCommentLocalPath(cPath, i + 1, AskRedditXPaths.commentLocal);

        private static readonly int baseLength = "padding-left: ".Length;

        /// <returns> if returns -1: it means target element is not a comment, but button that loads another comments </returns>
        public static int GetCommentLayer(IWebElement e)
        {
            string padding = e.GetAttribute("style"); // returns: "padding-left: 37px;"

            if (padding.Length == 0) return -1;

            string fV = padding.Substring(baseLength, padding.Length - baseLength - 3);

            switch (fV)
            {
                case ("16"): return 0;
                case ("37"): return 1;
                case ("58"): return 2;
                case ("79"): return 3;
                case ("100"): return 4;
                default: return 99;
            }
        }

        /// <returns> if post is promoted</returns>
        public static bool PostIsPromoted(IWebElement post)
        {
            Debug.WriteLine("Is promoted ?");

            try { post.FindElement(By.ClassName(AskRedditXPaths.post_promotedClass)); }
            catch { return false; }

            return true;
        }

        public static string CommentPath(string cPath, int id) => $"{cPath}[{id}]";

        /// <returns> comment text (with pauses ?) without hyperlinks, no words are replaced </returns>
        public static string GetCommentText(IWebElement cLocal, bool includePauses = false)
        {
            ReadOnlyCollection<IWebElement> paragraphs = cLocal.FindElements(By.TagName("p"));

            string t = "";

            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (includePauses)
                {
                    string lPause = i == paragraphs.Count - 1 ? "" : RedditSurfer.linePause;

                    t += $"{paragraphs[i].Text} {lPause} ";
                }
                else t += $"{paragraphs[i].Text}";
            }

            return TextUtils.RemoveHyperlinks(t);
        }

        /// <returns> if comment can be used for video (not mod comment & not too big & not a reply) </returns>
        public static bool CommentIsSuitable(string cPath, int i)
        {
            if (i == 0) // bot comments should be the first (maybye - I hope)
            {
                if (IsModeratorComment(cPath, i + 1)) return false;
            }

            IWebElement e = GetElementOnCommentLocalPath(cPath, i + 1, AskRedditXPaths.commentPosition);

            if (e.Size.Height > 750) return false; // comment is too big and wouldn't fit into the screenshot

            if (GetCommentLayer(e) != 0) return false; // if comment is a reply to another post

            return true;
        }

        /// <summary> Takes screenshot of 'element' and saves it under 'Paths.filesPath' as 'name'.png </summary>
        /// <param name="x"> Offset in x direction (horizontal) </param>
        public static void TakeScreenshot(IWebElement element, string name, int x)
        {
            string fullName = $"{name}.png";
            string fileName = @$"{Paths.filesPath}{fullName}";

            byte[] byteArray = ((ITakesScreenshot)RedditSurfer.driver).GetScreenshot().AsByteArray;
            Rectangle croppedImage = new Rectangle(element.Location.X - x / 2, element.Location.Y - x / 2, element.Size.Width + x, element.Size.Height + x);

            Bitmap screenshot = new Bitmap(new MemoryStream(byteArray));

            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);

            screenshot.Save(String.Format(fileName, ImageFormat.Png));
        }

        public static string GetPostPath(int sPost) => $"{AskRedditXPaths.posts}[{sPost}]";

        public static IWebElement GetElementOnCommentLocalPath(string cPath, int id, string path) => Base.GetElement_X($"{CommentPath(cPath, id)}/{path}");
        public static IWebElement? TryGetElementOnCommentLocalPath(string cPath, int id, string path) => Base.TryGetElement($"{CommentPath(cPath, id)}/{path}");
    }
}