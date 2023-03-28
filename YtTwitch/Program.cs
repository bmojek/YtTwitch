using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Net;

List<string> ListaUrl = new List<string>();
List<string> TempListaUrl = new List<string>();

static void DownloadUrls(List<string> listaUrl)
{
    var driver = new ChromeDriver();

    string url = "https://streamscharts.com/clips?time=7-days&language=pl";

    driver.Navigate().GoToUrl(url);

    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    driver.Manage().Window.Maximize();

    System.Threading.Thread.Sleep(1000);
    IReadOnlyCollection<IWebElement> clips = wait.Until(i => i.FindElements(By.ClassName("clip-container")));
    int i = 0;
    foreach (IWebElement element in clips)
    {
        if (i > 11)
        {
            Actions actions = new Actions(driver);
            actions.ScrollByAmount(0, 200);
            actions.Perform();
            i = 0;
        }
        System.Threading.Thread.Sleep(800);
        element.Click();
        System.Threading.Thread.Sleep(800);
        IReadOnlyCollection<IWebElement> Dbtn = wait.Until(i => i.FindElements(By.XPath("/html/body/main/div[3]/div/div[3]/div/div[2]/div[1]/div/a")));
        if (!listaUrl.Contains(Dbtn.First().GetAttribute("href"))) listaUrl.Add(Dbtn.First().GetAttribute("href"));
        IReadOnlyCollection<IWebElement> Cbtn = wait.Until(i => i.FindElements(By.XPath("//button[@class = 'absolute top-0 right-0 z-30 flex items-center justify-center w-10 h-10 focus:outline-none']")));
        Cbtn.First().Click();
        i++;
    }
    driver.Close();
}

/*DownloadUrls(ListaUrl);
int i = 0;
foreach (var x in ListaUrl)
{
    if (!TempListaUrl.Contains(x))
    {
        using (var client = new WebClient())
        {
            client.DownloadFile(x, $"clip{i++}.mp4");
            TempListaUrl.Add(x);
        }
    }
}*/