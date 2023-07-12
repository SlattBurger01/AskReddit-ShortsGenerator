using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ProjectCarrot
{
    public static class WebScraperBase
    {
        public static IWebDriver driver; // set once on setup

        public const int waitPause = 50; // in ms
        public const int maxWaitTime = 5000; // in ms
        public static readonly int maxUploadInterWaitTime = 600000; // in ms, used for wait times that relies on converting or uploading stuff

        /// <returns> Default chrome options </returns>
        public static ChromeOptions GetDefaultOptions()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--disable-notifications");
            options.AddArguments("disable-infobars");

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            options.AddUserProfilePreference("download.default_directory", LocalPaths.filesPath);

            return options;
        }

        /// <summary> Sends 'keys' to element on 'xPath' </summary>
        public static void SendKeysToElement(string xPath, string keys) => GetElement_X(xPath).SendKeys(keys);

        /// <summary> Clics element on xPath </summary>
        public static void ClickElement(string xPath) => GetElement_X(xPath).Click();

        /// <returns> Element on 'xPath' </returns>
        public static IWebElement GetElement_X(string xPath) => driver.FindElement(By.XPath(xPath));

        /// <returns> Collection of elements (if found), or empty one </returns>
        public static ReadOnlyCollection<IWebElement> TryGetElements(string xPath)
        {
            try { return GetElements_X(xPath); }
            catch { }

            return new List<IWebElement>().AsReadOnly();
        }

        /// <returns> Collection of elements of 'xPath' </returns>
        public static ReadOnlyCollection<IWebElement> GetElements_X(string xPath) => driver.FindElements(By.XPath(xPath));

        /// <summary> Clicks element on target path, if error is thrown --> it will be ignored </summary>
        public static bool TryClickElement(string xPath)
        {
            try { ClickElement(xPath); return true; } catch { return false; }
        }

        /// <returns> If element exists </returns>
        public static bool ElementExists(string xPath) => ElementExists(xPath, out _);

        /// <returns> If element exists </returns>
        public static bool ElementExists(string xPath, out IWebElement? element)
        {
            try { element = GetElement_X(xPath); return true; }
            catch { element = null; return false; }
        }

        /// <returns> Element (if exists, otherwise null) </returns>
        public static IWebElement? TryGetElement(string xPath)
        {
            try { return GetElement_X(xPath); }
            catch { return null; }
        }

        /// <summary> Waits for element (to exists and be clickable) and clicks it </summary>
        public static void WaitAndClickElement(string xPath, int maxWaitTime_ = maxWaitTime)
        {
            IWebElement? e = WaitForElement(xPath, maxWaitTime_);

            if (e == null) throw new NullReferenceException("Element was not found!");
            else WaitForElementBeClickableAndClick(e);
        }

        /// <summary> Waits until element is clickable and clics it </summary>
        public static void WaitForElementBeClickableAndClick(string xPath, int maxWaitTime_ = maxWaitTime) => WaitForElementBeClickableAndClick(GetElement_X(xPath), maxWaitTime_);

        /// <summary> Waits until element is clickable and clics it </summary>
        public static void WaitForElementBeClickableAndClick(IWebElement element, int maxWaitTime_ = maxWaitTime)
        {
            bool b = WaitForElementBeClickable(element, maxWaitTime_);

            if (b) element.Click();
        }

        /// <summary> Waits until element is clickable </summary>
        public static bool WaitForElementBeClickable(string xPath, int maxWaitTime_ = maxWaitTime) => WaitForElementBeClickable(GetElement_X(xPath), maxWaitTime_);

        /// <summary> Waits until element is clickable </summary>
        public static bool WaitForElementBeClickable(IWebElement element, int maxWaitTime_ = maxWaitTime)
        {
            int elapsed = 0; // in ms

            while (true)
            {
                bool isClickable = element.Displayed && element.Enabled;

                if (isClickable) return true;

                Thread.Sleep(waitPause);
                elapsed += waitPause;

                if (elapsed >= maxWaitTime_) return false;
            }
        }

        /// <summary> Waits until element exists and send 'keys' </summary>
        public static void WaitAndSendKeysToElement(string xPath, string keys, int maxWaitTime_ = maxWaitTime)
        {
            IWebElement? e = WaitForElement(xPath, maxWaitTime_);

            if (e == null) throw new NullReferenceException("Element was not found!");
            else e.SendKeys(keys);
        }

        /// <summary> Waits until element exists (on path) or until it max wait time is exeeded </summary>
        public static IWebElement? WaitForElement(string xPath, int maxWaitTime_ = maxWaitTime)
        {
            Debug.WriteLine($"Waiting for element (max = {maxWaitTime_ / 1000}s)");

            int elapsed = 0; // in ms

            while (elapsed < maxWaitTime_)
            {
                if (ElementExists(xPath, out IWebElement? e)) return e;

                Thread.Sleep(waitPause);
                elapsed += waitPause;
            }

            return null;
        }

        /// <summary> Waits until element disappears (on path) or until it max wait time is exeeded </summary>
        public static void WaitForElementDisappear(string xPath, int maxWaitTime_ = maxWaitTime)
        {
            int elapsed = 0;

            while (ElementExists(xPath))
            {
                Thread.Sleep(50);
                elapsed += 50;

                if (elapsed >= maxWaitTime_) return;
            }
        }

        /// <summary> Accepts alert, if error is thrown --> it will be ignored </summary>
        public static void TryAcceptAlert()
        {
            try { driver.SwitchTo().Alert().Accept(); } catch { }
        }

        /// <summary> Dismisses alert, if error is thrown --> it will be ignored </summary>
        public static void TryDismissAlert()
        {
            try { driver.SwitchTo().Alert().Dismiss(); } catch { }
        }

        /// <summary> instantly scrolls to element </summary>
        public static void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'nearest'});", element);
        }

        /// <summary> instantly scrolls to top </summary>
        public static void ScrollToTop()
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
        }

        /// <summary> Opens url in new window (chrome has to be already started) </summary>
        public static void OpenUrlInNewWindow(string url)
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl(url);
        }

        /// <summary> Clicks on element using IJavaScriptExecutor </summary>
        public static void ForcedClick(string path) => ForcedClick(GetElement_X(path));

        /// <summary> Clicks on element using IJavaScriptExecutor </summary>
        public static void ForcedClick(IWebElement element)
        {
            (driver as IJavaScriptExecutor).ExecuteScript("arguments[0].click();", element);
        }

        public static ReadOnlyCollection<IWebElement> GetElementsOnOneOfPaths(string[] paths, int minLength, out string usedPath)
        {
            usedPath = "";
            ReadOnlyCollection<IWebElement> array = new List<IWebElement>().AsReadOnly();

            for (int i = 0; i < paths.Length; i++)
            {
                array = GetElements_X(usedPath = paths[i]);

                if (array.Count >= minLength) return array;
            }

            return array;
        }
    }
}