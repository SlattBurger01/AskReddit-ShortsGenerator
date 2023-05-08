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
}