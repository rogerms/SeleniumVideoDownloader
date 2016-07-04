using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;

namespace SeleniumVideoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting...");

            Dictionary<string,string> links =  getVideoLinks();
            int count = 0;
            Dictionary<string, string> missing = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> link in links)
            {
                if (!fileExists(link.Value))
                {
                    count++;
                    Console.WriteLine(link.Value);
                    missing.Add(link.Key, link.Value);
                }
                 //downloadVideo(link.Key);
            }
            Console.WriteLine("Total missing: " + count);

            //downloadVideo(missing);
            Console.Read();
        }

        static bool fileExists(string  name)
        {
            DirectoryInfo d = new DirectoryInfo(@"C:\Users\Roger\Downloads");
            return d.GetFiles(name + "*").Length > 0;
        }

        static Dictionary<string, string> getVideoLinks()
        {
            IWebDriver driver;
            Dictionary<string, string> links = new Dictionary<string, string>();
            driver = new ChromeDriver(@"C:\Extensions");

            string url = "http://gryllus.net/Blender/VideoTutorials/AllVideoTutorials.html";
            driver.Navigate().GoToUrl(url);

            Console.WriteLine("Page Loading...");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.UrlToBe(url));
            Console.WriteLine("Page Loaded");

            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath("(//a[contains(@href,'vimeo.com')])"));

            foreach (IWebElement element in elements)
            {
                string key = element.GetAttribute("href").Replace("http:", "https:");
                if(!links.ContainsKey(key))
                    links.Add(key, element.Text.Replace("/", "").Replace(" ",""));
            }

            Console.WriteLine("Total links: " + links.Count);
            foreach (KeyValuePair<string, string> link in links)
            {
                Console.WriteLine(link.Key+" "+link.Value);
            }

            try
            {
                driver.Quit();
                Console.WriteLine("Quit");
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            return links;
        }

        static void downloadVideo(Dictionary<string, string> links)
        {

            FirefoxProfile firefoxProfile = new FirefoxProfile();
            ////not allowed 
            ////firefoxProfile.SetPreference("browser.download.manager.showWhenStarting", false);
            firefoxProfile.SetPreference("browser.download.dir", "c:\\downloads");
            firefoxProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "video/mp4");

            // pdf forcing download specialProfile.SetPreference("pdfjs.disabled", true); // for my Firefox 20.0

           

            IWebDriver driver;
            string baseURL = "https://vimeo.com/";

            //driver = new FirefoxDriver();
            //driver = new FirefoxDriver(firefoxProfile);
            driver = new ChromeDriver(@"C:\Extensions");


            foreach (KeyValuePair<string, string> iurl in links)
            {
                string url = iurl.Key;
                Console.WriteLine(url);
 
                driver.Navigate().GoToUrl(url);

                Console.WriteLine("Page Loading...");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.UrlToBe(url));
                Console.WriteLine("Page Loaded");

                Console.WriteLine("Download button");
                IWebElement myDynamicElement = wait.Until<IWebElement>(d => d.FindElement(By.XPath("(//span[text()='Download'])")));
                driver.FindElement(By.XPath("(//span[text()='Download'])")).Click();
                // <span class="iris_btn-content">Download</span>
                Console.WriteLine("Download button clicked");

                Thread.Sleep(10000);
                Console.WriteLine("Choose download...");
                myDynamicElement = wait.Until<IWebElement>(d => d.FindElement(By.XPath("(//a[text()='Download'])[2]")));
                if (myDynamicElement != null)
                {
                    var newElement = wait.Until(ExpectedConditions.ElementToBeClickable(myDynamicElement));
                    if (newElement != null)
                    {
                        myDynamicElement.Click();
                        Console.WriteLine("Choose download click");
                    }
                }

                Console.WriteLine("Close dialog");
                Thread.Sleep(5000);
                //try
                //{
                //    myDynamicElement = wait.Until<IWebElement>(d => d.FindElement(By.CssSelector("button.iris_modal-btn--close")));
                //    if (myDynamicElement != null)
                //    {
                //        myDynamicElement.Click();
                //        Console.WriteLine("Close dialog click");
                //    }
                //    Thread.Sleep(5000);
                //}
                //catch(Exception) { }
            }
            //wait for download to finish
            Thread.Sleep(15000);

            try
            {
                driver.Quit();
                Console.WriteLine("Quit");
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        private static bool IsElementPresent(By by, IWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
