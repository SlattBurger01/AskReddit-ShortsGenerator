using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProjectCarrot.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using Base = ProjectCarrot.WebScraperBase;

namespace ProjectCarrot
{
    public static class RedditSurferBase
    {
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

        /// <summary> Tries to open post, if fails: tries to close interest tab </summary>
        /// <returns> if post was opened </returns>
        public static bool OpenPost_(int postId, bool forced)
        {
            try { OpenPostF(postId, forced); }
            catch (Exception e)
            {
                IWebElement element = Base.GetElement_X(GetPostPath(postId));

                if (element.Size.Height == 0) return false; // fcked up comment 

                Debug.WriteLine($"Post could not be opened! {e.Message}");

                Base.ClickElement(AskRedditXPaths.interestsTab);
                //Thread.Sleep(100);
                OpenPostF(postId + 2, forced);
            }

            return true;
        }

        /// <param name="cPath"> used path </param>
        /// <returns> found comments </returns>
        public static ReadOnlyCollection<IWebElement> GetComments(out string cPath)
        {
            return Base.GetElementsOnOneOfPaths(AskRedditXPaths.commentsArray, 2, out cPath);
        }

        /// <returns> If comments were loaded (25) or waited more than 4 secs, but found at least two (true), otherwise if comments were not (at least 2) loaded in 1 second or waited longer than 'Base' allows </returns>
        public static bool WaitForCommentsToLoad(out ReadOnlyCollection<IWebElement> comments, out string cPath)
        {
            float elapsedTime = 0;

            while (true)
            {
                comments = GetComments(out cPath);

                if (elapsedTime >= 4000 && comments.Count < 25) return comments.Count >= 2;
                if (elapsedTime >= 1000 && comments.Count < 2) return false;

                if (comments.Count >= 25) return true;

                Thread.Sleep(Base.waitPause);
                elapsedTime += 50;

                if (elapsedTime > Base.maxWaitTime) return false;
            }
        }

        public static IWebElement GetCommentsTextParent(string cPath, int i) => GetElementOnCommentLocalPath(cPath, i + 1, AskRedditXPaths.commentLocal);

        private static readonly int baseLength = "padding-left: ".Length;

        private static void OpenPostF(int sPost, bool forced)
        {
            string path = $"{GetPostPath(sPost)}/{AskRedditXPaths.openPostButton_local}";

            Debug.WriteLine($"Opening post ({sPost}) at path ({path})");

            if (forced) Base.ForcedClick(path);
            else Base.ClickElement(path);

            Base.ScrollToTop(); // you have to reset position immidiatelly to make correct screenshots
        }

        public static void OpenNewRedditPage(string url)
        {
            Debug.WriteLine("Setting up reddit");

            Base.OpenUrlInNewWindow(url);

            Base.TryClickElement(AskRedditXPaths.acceptButton);

            if (Settings.loginToReddit) LoginToReddit();
        }

        // (it switches usually automatically), but this should worked on ( switch only if is in light mode )
        public static void TrySwitchToDarkMode()
        {
            IWebElement colorBackground = Base.GetElement_X("/html/body/div[1]/div");
            if (!colorBackground.GetAttribute("style").Contains("#FFFFFF")) return;

            IWebElement settings = Base.GetElement_X(AskRedditXPaths.settings);
            settings.Click();

            Base.ClickElement(AskRedditXPaths.darkMode);

            settings.Click();
        }

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

        /// <summary> Can be used for post description as well </summary>
        /// <returns> Comment text (with pauses ?) without hyperlinks, no words are replaced </returns>
        public static string GetCommentText(IWebElement cLocal, bool includePauses = false)
        {
            ReadOnlyCollection<IWebElement> paragraphs = cLocal.FindElements(By.TagName("p"));

            string t = "";

            for (int i = 0; i < paragraphs.Count; i++)
            {
                string space = i != paragraphs.Count - 1 ? " " : "";

                string pText = paragraphs[i].GetAttribute("textContent");

                if (includePauses)
                {
                    Debug.WriteLine($"pText {i}: {paragraphs[i].Text}, {pText}");

                    bool addPause = i != paragraphs.Count - 1 && pText.Last() != '.';

                    string lPause = addPause ? AskRedditSurfer.linePause : "";

                    t += $"{pText}{lPause}{space}";
                }
                else t += $"{pText}{space}";
            }

            return TextUtils.RemoveHyperlinks(t);
        }

        /// <returns> if comment can be used for video (not mod comment & not too big & not a reply) </returns>
        public static bool CommentIsSuitable(string cPath, int i, out IWebElement? commentPos)
        {
            commentPos = null;

            if (i == 0) // bot comments should be the first (maybye - I hope)
            {
                if (IsModeratorComment(cPath, i + 1)) return false;
            }

            commentPos = GetElementOnCommentLocalPath(cPath, i + 1, AskRedditXPaths.commentPosition);

            if (GetCommentLayer(commentPos) != 0) return false; // if comment is a reply to another post

            return true;
        }

        /// <summary> Takes screenshot of 'element' and saves it under 'Paths.filesPath' as 'name'.png </summary>
        /// <param name="x"> Offset in x direction (horizontal) </param>
        public static void TakeScreenshot(IWebElement element, string name, int x)
        {
            Rectangle croppedImage = new Rectangle(element.Location.X - x / 2, element.Location.Y - x / 2, element.Size.Width + x, element.Size.Height + x);
            TakeScreenshotF(name, croppedImage);
        }

        public static void TakeScreenshot(IWebElement element, string name, int xp, int xn, int yp, int yn)
        {
            Rectangle croppedImage = new Rectangle(element.Location.X - xn, element.Location.Y - yp, element.Size.Width + xp, element.Size.Height + yn);

            TakeScreenshotF(name, croppedImage);
        }

        private static void TakeScreenshotF(string name, Rectangle croppedImg)
        {
            string fullName = $"{name}.png";
            string fileName = @$"{LocalPaths.filesPath}{fullName}";

            byte[] byteArray = ((ITakesScreenshot)AskRedditSurfer.driver).GetScreenshot().AsByteArray;

            Bitmap screenshot = new Bitmap(new MemoryStream(byteArray));

            screenshot = screenshot.Clone(croppedImg, screenshot.PixelFormat);

            screenshot.Save(String.Format(fileName, ImageFormat.Png));
        }

        /// <returns> Path of posts of 'id' </returns>
        public static string GetPostPath(int id) => $"{AskRedditXPaths.posts}[{id}]";

        /// <returns> Returns element on local path under comment on 'cPath' with 'id' </returns>
        public static IWebElement GetElementOnCommentLocalPath(string cPath, int id, string path) => Base.GetElement_X($"{CommentPath(cPath, id)}/{path}");

        /// <returns> Returns element on local path under comment on 'cPath' with 'id' (if exists) </returns>
        public static IWebElement? TryGetElementOnCommentLocalPath(string cPath, int id, string path) => Base.TryGetElement($"{CommentPath(cPath, id)}/{path}");

        /// <summary> Logins to reddit :) </summary>
        public static void LoginToReddit()
        {
            bool shreddit = false;

            if (!Base.TryClickElement(AskRedditXPaths.loginButton_old)) shreddit = true;

            if (shreddit)
            {
                Base.ClickElement(AskRedditXPaths.loginButton_new);

                Base.SendKeysToElement(AskRedditXPaths.usernameInput, LogingData.userNameReddit);
                Base.SendKeysToElement(AskRedditXPaths.passwordInput, LogingData.password);

                Base.ClickElement(AskRedditXPaths.finalLoginButton);
            }
            else
            {
                Base.WaitForElement(AskRedditXPaths.loginIFrame);

                AskRedditSurfer.driver.SwitchTo().Frame(0);

                Base.SendKeysToElement(AskRedditXPaths.usernameInput, LogingData.userNameReddit);
                Base.SendKeysToElement(AskRedditXPaths.passwordInput, LogingData.password);

                Base.ClickElement(AskRedditXPaths.finalLoginButton);
            }
        }

        public static ChromeDriver SetupDriverForVideoGeneration()
        {
            bool installAddblock = Settings.speechType == TextToSpeechType.ttsfree;

            ChromeOptions options = Base.GetDefaultOptions();

            // disable "save password" popup
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddArgument("mute-audio");

            if (installAddblock) options.AddExtension(LocalPaths.adAwayAddBlock);

            ChromeDriver driver = new ChromeDriver(options);
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

            return driver;
        }

        public static string GetPostDescription()
        {
            // this element should exist in every post (in some of them it is just empty)
            IWebElement? el = Base.WaitForElement(AskRedditXPaths.postDescriptionParent_4);

            string t = "";

            if (el != null)
            {
                if (el.Size.Height > 1) t = GetCommentText(el, true);
            }

            if (t == "")
            {
                t  = GetCommentText(Base.GetElement_X(AskRedditXPaths.postDescriptionParent_5), true);
            }

            return t;
        }
    }
}