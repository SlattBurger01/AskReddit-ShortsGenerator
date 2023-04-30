using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.Extensions;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace ProjectCarrot
{
    public static class WebScraperBase
    {
        public static IWebDriver driver; // set once on setup

        public static ChromeOptions GetDefaultOptions()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--disable-notifications");
            options.AddArguments("disable-infobars");

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            options.AddUserProfilePreference("download.default_directory", Paths.filesPath);

            return options;
        }

        public static void SendKeysToElement(string xPath, string keys) => GetElement_X(xPath).SendKeys(keys);

        public static void ClickElement(string xPath) => GetElement_X(xPath).Click();

        public static IWebElement GetElement_X(string xPath) => driver.FindElement(By.XPath(xPath));
        public static ReadOnlyCollection<IWebElement> GetElements_X(string xPath) => driver.FindElements(By.XPath(xPath));

        public static void TryClickElement(string xPath)
        {
            try { ClickElement(xPath); } catch { }
        }

        public static bool ElementExists(string xPath) => ElementExists(xPath, out _);

        public static bool ElementExists(string xPath, out IWebElement? element)
        {
            try { element = GetElement_X(xPath); return true; }
            catch { element = null; return false; }
        }

        public static IWebElement TryGetElement(string xPath)
        {
            try { return GetElement_X(xPath); }
            catch { return null; }
        }

        public static void WaitAndClickElement(string xPath)
        {
            IWebElement e = WaitForElement(xPath);
            e.Click();
        }

        private static readonly int waitPause = 50; // in ms

        public static IWebElement? WaitForElement(string xPath)
        {
            int overflow = 10_000; // in ms

            while (true)
            {
                if (ElementExists(xPath, out IWebElement? e)) return e;

                Thread.Sleep(waitPause);
                overflow -= waitPause;

                if (overflow <= 0) return null;
            }
        }

        public static void TryAcceptAlert()
        {
            try { driver.SwitchTo().Alert().Accept(); }
            catch { }
        }

        public static void TryDismissAlert()
        {
            try { driver.SwitchTo().Alert().Dismiss(); }
            catch { }
        }

        public static void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'nearest'});", element);
        }

        public static void ScrollToTop()
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
        }
    }

    public enum TextToSpeechType { ttsmp3, ttsfree, pyttsx3 }

    public static class Settings
    {
        // for "Create" and "Upload" option
        public static readonly SessionSettings defaultSettings = new SessionSettings("def", 5, RedditUrls.AskReddit, ChromePersons.person2, new UploadPlatforms(1, 1, 0));

        // for "Create and Upload" option
        public static readonly SessionSettings[] sessionsSettings = new SessionSettings[] 
        {
            new SessionSettings("p1", 4, RedditUrls.AskWomen, ChromePersons.person1, new UploadPlatforms(1, 1, 0)),
            new SessionSettings("p2", 4, RedditUrls.AskReddit, ChromePersons.person2, new UploadPlatforms(1, 1, 0)),
            new SessionSettings("p3", 4, RedditUrls.AskMen, ChromePersons.person3, new UploadPlatforms(1, 1, 0))
        };

        public static readonly int sessionSettingsVideoCount = GetSessionSettingsVideoCount(sessionsSettings);

        public static readonly TextToSpeechType speechType = TextToSpeechType.ttsfree;

        public static readonly bool loginToReddit = true;
        public static readonly bool loginToTtsFree = true;

        public static readonly int maxWordCountPerComment = int.MaxValue;
        public static readonly int maxCharsCountPerComment = 2000;

        public static readonly int schedulePause = 4; // in 1/4 hours

        // debug optios
        public static readonly bool renderVideo = true;
        public static readonly bool readText = true;

        public static int GetSessionSettingsVideoCount(SessionSettings[] settings)
        {
            int val = 0;

            for (int i = 0; i < settings.Length; i++)
            {
                val += settings[i].videoCount;
            }

            return val;
        }
    }

    public struct SessionSettings
    {
        public string redditUrl;
        public string uploadPerson;
        public UploadPlatforms uploadPlatforms;
        public int videoCount;
        public string sessionName;

        public SessionSettings(string name, int c, string rURL, string person, UploadPlatforms platforms)
        {
            redditUrl = rURL;
            uploadPerson = person;
            uploadPlatforms = platforms;
            videoCount = c;
            sessionName = name;
        }
    }

    public struct UploadPlatforms
    {
        public bool includeYoutube;
        public bool includeTikTok;
        public bool includeInstagram;

        public UploadPlatforms(bool y, bool t, bool i)
        {
            includeYoutube = y;
            includeTikTok = t;
            includeInstagram = i;
        }

        public UploadPlatforms(int y, int t, int i)
        {
            includeYoutube = y == 1;
            includeTikTok = t == 1;
            includeInstagram = i == 1;
        }
    }
}