using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Base = ProjectCarrot.WebScraperBase;

namespace ProjectCarrot
{
    public static class TextReader
    {
        public static void Test()
        {

        }

        public static void SetUpReader(TextToSpeechType speechType, ChromeDriver driver)
        {
            switch (speechType)
            {
                case TextToSpeechType.ttsmp3:
                    driver.Navigate().GoToUrl("https://ttsmp3.com/");
                    Base.ClickElement(Ttsmp3XPaths.mathewVoice);
                    break;

                case TextToSpeechType.ttsfree:
                    string url = Settings.loginToTtsFree ? "https://ttsfree.com/login" : "https://ttsfree.com/";

                    driver.Navigate().GoToUrl(url);

                    Thread.Sleep(500);

                    Base.ClickElement("/html/body/div[1]/div/div/div/div[2]/div/button[2]"); // cookies

                    Thread.Sleep(200);

                    if (Settings.loginToTtsFree) LoginToTtsFree();

                    Base.WaitAndClickElement("/html/body/section[2]/div[2]/form/div[2]/div[1]/div[1]/div[2]/div/div[4]"); // select target voice

                    IWebElement speedSlider = Base.GetElement_X("/html/body/section[2]/div[2]/form/div[2]/div[1]/div[1]/span");

                    Actions action = new Actions(driver);
                    action.MoveToElement(speedSlider);
                    action.ClickAndHold().MoveByOffset(30, 0).Release(); // speed up voice by ( 10 = 13% | 15 = 20% | 20 = 26% | 30 = 40%
                    action.Build().Perform();

                    break;

                case TextToSpeechType.pyttsx3: break;
            }
        }

        private static void LoginToTtsFree()
        {
            Base.SendKeysToElement(TtsfreeXPaths.usernameInput, LogingData.userNameTtsFree);
            Base.SendKeysToElement(TtsfreeXPaths.passwordInput, LogingData.password);

            Base.ClickElement(TtsfreeXPaths.agreeButton);

            Base.ClickElement(TtsfreeXPaths.submitButton);
        }

        public static void ReadText(string text, string fileName, TextToSpeechType speechType)
        {
            if (!Settings.readText) return;

            string fText_ = text.Replace('"'.ToString(), string.Empty); // so it does not mess up args for python text to speech

            string fText = Regex.Replace(fText_, @"\p{Cs}", "");

            string fixedText = ExplicitWordHandler.ReplaceExplicitWords(fText);

            switch (speechType)
            {
                case TextToSpeechType.ttsmp3:
                    ReadText_Ttsmp3(fixedText, fileName);
                    break;

                case TextToSpeechType.ttsfree:
                    ReadText_Ttsfree(fixedText, fileName);
                    break;

                case TextToSpeechType.pyttsx3:
                    ReadText_Pyttsx3(fixedText, fileName);
                    break;
            }
        }

        private static readonly char s = '"';

        private static void ReadText_Pyttsx3(string text, string fileName)
        {
            string r = PythonHelper.CallPythonFile(Paths.customReaderPath, $"{s}{text}{s} {fileName} {Paths.filesPath}");

            Debug.WriteLine(r);
        }

        private static void ReadText_Ttsmp3(string text, string fileName)
        {
            RedditSurfer.OpenReader();

            Debug.WriteLine(text);

            Thread.Sleep(2000);

            IWebElement input = Base.GetElement_X(Ttsmp3XPaths.input);

            input.Clear();
            Thread.Sleep(100);
            input.Clear();

            try { input.SendKeys(text); }
            catch
            {
                string t1 = ConvertToBMP(text);

                Debug.WriteLine($"CONVERTING: {text} TO {t1}");

                input.SendKeys(t1);
            }

            Base.ClickElement(Ttsmp3XPaths.downloadButton);

            RedditSurfer.targetnames.Add(fileName);
            RedditSurfer.OpenReddit();
        }

        private static void ReadText_Ttsfree(string text, string fileName)
        {
            Debug.WriteLine(text);

            RedditSurfer.OpenReader();

            IWebElement input;

            try
            {
                input = Base.GetElement_X(TtsfreeXPaths.textInput);
            }
            catch // another download page was opened
            {
                WebScraperBase.driver.Navigate().Back();
                input = Base.GetElement_X(TtsfreeXPaths.textInput);
            }

            input.Clear();
            input.SendKeys(text);

            Base.ClickElement(TtsfreeXPaths.converButton); // convert

            while (Base.ElementExists(TtsfreeXPaths.downloadButton)) // wait until the it starts converting so you don't download the old audio
            {
                Debug.WriteLine("Waiting");
                Thread.Sleep(25);
            }

            Base.WaitAndClickElement(TtsfreeXPaths.downloadButton); // download

            RedditSurfer.targetnames.Add(fileName);
            RedditSurfer.OpenReddit();
        }

        public static string ConvertToBMP(string s1)
        {
            string s2 = "";

            for (int i = 0; i < s1.Length; i++)
            {
                char ch = s1[i];
                if (!char.IsSurrogate(ch)) s2 += ch;
            }

            return s2;
        }
    }
}