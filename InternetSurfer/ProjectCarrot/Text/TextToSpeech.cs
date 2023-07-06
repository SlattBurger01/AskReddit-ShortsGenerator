using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using Base = ProjectCarrot.WebScraperBase;

namespace ProjectCarrot
{
    public static class TextReader
    {
        public static void SetUpReader(TextToSpeechType speechType, ChromeDriver driver)
        {
            switch (speechType)
            {
                case TextToSpeechType.ttsmp3:
                    driver.Navigate().GoToUrl("https://ttsmp3.com/");
                    Base.ClickElement(Ttsmp3XPaths.mathewVoice);
                    break;

                case TextToSpeechType.narakeet:
                    driver.Navigate().GoToUrl("https://www.narakeet.com/app/text-to-audio");
                    break;

                case TextToSpeechType.ttsfree:
                    string url = Settings.loginToTtsFree ? "https://ttsfree.com/login" : "https://ttsfree.com/";
                    driver.Navigate().GoToUrl(url);

                    /*Thread.Sleep(500);

                    Base.ClickElement("/html/body/div[1]/div/div/div/div[2]/div/button[2]"); // cookies */

                    Base.WaitAndClickElement("/html/body/div[1]/div/div/div/div[2]/div/button[2]"); // cookies

                    //Thread.Sleep(200);

                    if (Settings.loginToTtsFree) LoginToTtsFree();

                    Base.WaitAndClickElement("/html/body/section[2]/div[2]/form/div[2]/div[1]/div[1]/div[2]/div/div[4]"); // select target voice

                    IWebElement speedSlider = Base.GetElement_X("/html/body/section[2]/div[2]/form/div[2]/div[1]/div[1]/span"); // Jake voice

                    Actions action = new Actions(driver);
                    action.MoveToElement(speedSlider);

                    // if chrome is being moved while adjusting voice speed, the value might be diferent
                    action.ClickAndHold().MoveByOffset((int)TtsFreeSpeed.p14, 0).Release();
                    action.Build().Perform();

                    break;

                case TextToSpeechType.voiceMaker:
                    driver.Navigate().GoToUrl("https://voicemaker.in/");

                    Base.TryClickElement("/html/body/nav/div/button"); // menu expand button

                    Base.ClickElement("/html/body/nav/div/div/ul/div[1]/button"); // login

                    Base.SendKeysToElement("/html/body/div[1]/div/div/div/div/form/div[2]/input", LogingData.gmailVoiceMaker); // gmail input
                    Base.SendKeysToElement("/html/body/div[1]/div/div/div/div/form/div[3]/input", LogingData.password); // password input

                    Base.ClickElement("/html/body/div[1]/div/div/div/div/form/button"); // login final

                    break;

                case TextToSpeechType.pyttsx3: break;
            }
        }

        public static void ReadText(string text, string fileName, TextToSpeechType speechType, out string fText)
        {
            fText = "";

            if (!Settings.readText) return;

            fText = TextsManager.GetFixedText(text);

            Debug.WriteLine($"Reading text: {text}");

            switch (speechType)
            {
                case TextToSpeechType.ttsmp3:
                    ReadText_Ttsmp3(fText, fileName);
                    break;

                case TextToSpeechType.narakeet:
                    ReadText_Narakeet(fText, fileName);
                    break;

                case TextToSpeechType.ttsfree:
                    ReadText_Ttsfree(fText, fileName);
                    break;

                case TextToSpeechType.voiceMaker:
                    RedText_VoiceMaker(fText, fileName);
                    break;

                case TextToSpeechType.pyttsx3:
                    ReadText_Pyttsx3(fText, fileName);
                    break;
            }
        }

        private static void LoginToTtsFree()
        {
            Base.SendKeysToElement(TtsfreeXPaths.usernameInput, LogingData.userNameTtsFree);
            Base.SendKeysToElement(TtsfreeXPaths.passwordInput, LogingData.password);

            Base.ClickElement(TtsfreeXPaths.agreeButton);

            Base.ClickElement(TtsfreeXPaths.submitButton);
        }

        private static void RedText_VoiceMaker(string text, string fileName)
        {
            RedditSurfer.OpenReader();

            RedditSurfer.driver.Navigate().Refresh(); // necesary to determine if audio was already converted

            /* Jake voice*/
            try { Base.WaitAndClickElement("/html/body/section/div/div/div/form/div[4]/div[2]/div[2]/div[2]/label"); }
            catch (StaleElementReferenceException) { Base.WaitAndClickElement("/html/body/section/div/div/div/form/div[4]/div[2]/div[2]/div[2]/label"); }

            IWebElement element = Base.GetElement_X("/html/body/section/div/div/div/form/div[1]/div[1]/div[2]/div[1]/textarea"); // input
            element.Clear();
            element.SendKeys(text);

            Base.ClickElement("/html/body/section/div/div/div/form/div[4]/div[3]/div[1]/button[1]"); // convert button

            while (true)
            {
                IWebElement e = Base.GetElement_X("/html/body/section/div/div/div/form/div[3]/div");

                if (e.GetAttribute("style") == "display: block;") break;

                Thread.Sleep(50);
            }

            Base.ClickElement("/html/body/section/div/div/div/form/div[4]/div[3]/div[1]/button[2]"); // download

            AudioFilesHandler.AddTargetName(fileName);
            RedditSurfer.OpenReddit();
        }

        // Narakeet does not support comercial use for free accounts, so maybye be careful :)
        private static void ReadText_Narakeet(string text, string fileName)
        {
            RedditSurfer.OpenReader();

            IWebElement input = Base.GetElement_X("/html/body/main/div[5]/div/div[1]/div[3]/div[6]/div[2]");

            input.Clear();
            input.SendKeys(text);

            Base.ClickElement("/html/body/main/div[5]/div/div[1]/div[6]/button[3]"); // create audio button

            Base.WaitAndClickElement("/html/body/main/div[4]/div/div/div[3]/a[3]", Base.maxUploadInterWaitTime); // download button, converting can take bit longer

            Base.WaitAndClickElement("/html/body/main/div[4]/div/div/div[3]/a[1]"); // "new audio button" (return)

            AudioFilesHandler.AddTargetName(fileName);
            RedditSurfer.OpenReddit();
        }

        private static void ReadText_Pyttsx3(string text, string fileName)
        {
            string r = PythonHelper.CallPythonFile(LocalPaths.customReaderPath, $"\"{text}\" {fileName} {LocalPaths.filesPath}");

            Debug.WriteLine(r);
        }

        private static void ReadText_Ttsmp3(string text, string fileName)
        {
            RedditSurfer.OpenReader();

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

            AudioFilesHandler.AddTargetName(fileName);
            RedditSurfer.OpenReddit();
        }

        private static void ReadText_Ttsfree(string text, string fileName)
        {
            RedditSurfer.OpenReader();

            IWebElement input;

            try
            {
                input = Base.GetElement_X(TtsfreeXPaths.textInput);
            }
            catch // another download page was opened
            {
                Base.driver.Navigate().Back();
                input = Base.GetElement_X(TtsfreeXPaths.textInput);
            }

            input.Clear();
            input.SendKeys(text);

            Base.ClickElement(TtsfreeXPaths.converButton); // convert

            while (Base.ElementExists(TtsfreeXPaths.downloadButton)) // wait until the button dissapears (it starts converting so you don't download the old audio)
            {
                Debug.WriteLine("Waiting");
                Thread.Sleep(50);
            }

            Base.WaitForElement(TtsfreeXPaths.downloadButton, Base.maxUploadInterWaitTime);

            IJavaScriptExecutor executor = Base.driver as IJavaScriptExecutor;
            executor.ExecuteScript("arguments[0].click();", Base.GetElement_X(TtsfreeXPaths.downloadButton));

            AudioFilesHandler.AddTargetName(fileName);
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

        // number after "p" or "n" is in %, "p" = positive, "n" = negative
        enum TtsFreeSpeed
        {
            p0 = 0,
            p14 = 10,
            p20 = 15,
            p26 = 20,
            p40 = 40
        }
    }
}