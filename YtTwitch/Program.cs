﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Reflection;
using System.Diagnostics;

List<string> ListaUrl = new List<string>();
List<string> Title = new List<string>();
List<string> StreamerName = new List<string>();


List<string> TempListaUrl = new List<string>()
{
    "https://clips-media-assets2.twitch.tv/9bSZhuJKmTcWYu95_DHMzg/AT-cm%7C9bSZhuJKmTcWYu95_DHMzg.mp4"

};

static void DownloadUrls(List<string> listaUrl, List<string> Title, List<string> StreamerName)
{
    var driver = new ChromeDriver();

    string url = "https://streamscharts.com/clips?time=7-days&language=pl";

    driver.Navigate().GoToUrl(url);

    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    driver.Manage().Window.Maximize();

    System.Threading.Thread.Sleep(1300);
    IReadOnlyCollection<IWebElement> clips = wait.Until(i => i.FindElements(By.ClassName("clip-container")));
    try
    {
        IReadOnlyCollection<IWebElement> Recap = wait.Until(i => i.FindElements(By.XPath("/html/body/main/div[2]/div/section/div/div/div/button")));
        Recap.First().Click();
    }
    catch { }
    
    int z = 0;
    foreach (IWebElement element in clips)
    {
        if (z %4 ==0)
        {
            Actions actions = new Actions(driver);
            actions.ScrollByAmount(0, 100);
            actions.Perform();
            if (z == 4)
            {
                try
                {
                    IReadOnlyCollection<IWebElement> Recap2 = wait.Until(i => i.FindElements(By.XPath("/html/body/main/div[2]/div/section/div/div/div/button")));
                    Recap2.First().Click();
                }
                catch { }
                
            }
            
        }
        System.Threading.Thread.Sleep(1300);
        element.Click();
        System.Threading.Thread.Sleep(1500);
        IReadOnlyCollection<IWebElement> Dbtn = wait.Until(i => i.FindElements(By.XPath("/html/body/main/div[4]/div/div[3]/div/div[2]/div[1]/div/a")));
        IReadOnlyCollection<IWebElement> DStreamerName = wait.Until(i => i.FindElements(By.XPath($"/html/body/main/div[4]/div/div[2]/div[1]/div[1]/div[3]/button[{z + 1}]/div[2]/div")));
        IReadOnlyCollection<IWebElement> DTitle = wait.Until(i => i.FindElements(By.XPath($"/html/body/main/div[4]/div/div[2]/div[1]/div[1]/div[3]/button[{z + 1}]/div[3]/div[2]/div/div[1]")));

        if (DStreamerName.First().Text != "AmadeuszFerrari") listaUrl.Add(Dbtn.First().GetAttribute("href"));
        else listaUrl.Add("AMADI");
        StreamerName.Add(DStreamerName.First().Text);
        Title.Add(DTitle.First().Text);

        IReadOnlyCollection<IWebElement> Cbtn = wait.Until(i => i.FindElements(By.XPath("//button[@class = 'absolute top-0 right-0 z-30 flex items-center justify-center w-10 h-10 focus:outline-none']")));
        Cbtn.First().Click();
        z++;
    }
    driver.Close();
}

static void ChangeResolution(string clipName)
{
    var ffmpeg = new NReco.VideoConverter.FFMpegConverter();
    // PRZYCIECIE "-filter:v \"crop=9/16*ih:ih\""
    var convertSettings = new NReco.VideoConverter.ConvertSettings()
    {
        CustomOutputArgs = "-vf \"scale=1080:1920:force_original_aspect_ratio=decrease,pad=1080:1920:-1:-1:color=black\""
    };
    var x = new NReco.VideoConverter.FFMpegInput[1] { new NReco.VideoConverter.FFMpegInput(clipName) };
    ffmpeg.ConvertMedia(x, $"Res{clipName}", "mp4", convertSettings);
}
int i = 0;
static void DownloadAndUpload(List<string> ListaUrl, List<string> Title, List<string> StreamerName, List<string> TempListaUrl, int i)
{
    DownloadUrls(ListaUrl, Title, StreamerName);

    foreach (var x in ListaUrl)
    {
        if (!TempListaUrl.Contains(x) && StreamerName[i] != "AmadeuszFerrari")
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(x, $"clip{i}.mp4");

                TempListaUrl.Add(x);

                ChangeResolution($"clip{i}.mp4");
            }
           new UploadVideo($"{StreamerName[i]} - {Title[i]} #shorts #twitchshorts #twitchshoty #twitch #irl #xayoo #franio", $"Resclip{i}.mp4").Run().Wait();

            break;
        }
        i++;
    }
}

Stopwatch Stopwatch = Stopwatch.StartNew();

while (true)
{
    if ((Stopwatch.Elapsed.TotalMilliseconds % 1000 == 0))
        Console.WriteLine("{0,3}\b\b\b", Stopwatch.Elapsed);

        

    if (Stopwatch.Elapsed.Seconds >= 4)
    {
        DownloadAndUpload(ListaUrl, Title, StreamerName, TempListaUrl, i);

        Stopwatch = Stopwatch.StartNew();
    }
}

public class UploadVideo
{
    public string Title { get; set; }
    public string FilePath { get; set; }

    public UploadVideo(string title, string filePath)
    {
        Title = title;
        FilePath = filePath;
    }

    static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
    public async Task Run()
    {
        UserCredential credential;
        using (var stream = new FileStream("secret.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None
            );
        }

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        });

        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = Truncate(Title,100);
        video.Snippet.Description = "";
        //video.Snippet.Tags = new string[] { };
        video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "public"; // or "private" or "public"
        var filePath = FilePath; // Replace with path to actual movie file.

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            await videosInsertRequest.UploadAsync();
        }
    }

    private void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
    {
        switch (progress.Status)
        {
            case UploadStatus.Uploading:
                Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                break;

            case UploadStatus.Failed:
                Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                break;
        }
    }

    private void videosInsertRequest_ResponseReceived(Video video)
    {
        Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
    }
}

