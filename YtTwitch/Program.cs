using OpenQA.Selenium;
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
    "https://clips-media-assets2.twitch.tv/cYi0O1Ps5Q4eHgQeDVruHw/AT-cm%7CcYi0O1Ps5Q4eHgQeDVruHw.mp4",
    "https://clips-media-assets2.twitch.tv/z9vF5BuLzYXpdO9K8z0M2A/39476779349-offset-11752.mp4",
    "https://clips-media-assets2.twitch.tv/Rp1lFD4JxOuQKf-qdea98g/AT-cm%7CRp1lFD4JxOuQKf-qdea98g.mp4",
    "https://clips-media-assets2.twitch.tv/L83SToGWbJzb8E46K8Bwyw/39476779349-offset-11292.mp4",
    "https://clips-media-assets2.twitch.tv/Mimmou44Ro2CK1t5AOd9yQ/39476779349-offset-10800.mp4",
    "https://clips-media-assets2.twitch.tv/voFJxEH4oI99jPitGh_BZA/39473284901-offset-4172.mp4",
    "https://clips-media-assets2.twitch.tv/8g9_9-sJ6TJ9Eww9xInvtg/39476779349-offset-14316.mp4",
    "https://clips-media-assets2.twitch.tv/LmtmdT_dj9rCDW2eVCRfWg/39476779349-offset-15082.mp4",
    "https://clips-media-assets2.twitch.tv/doo8S1flFk43kuCC2adp4A/AT-cm%7Cdoo8S1flFk43kuCC2adp4A.mp4",
    "https://clips-media-assets2.twitch.tv/icsjizMPcbTtkW2_G5M3zg/39485365685-offset-3702.mp4",
    "https://clips-media-assets2.twitch.tv/i2Mj6J9FVXdKDCF0owZynQ/AT-cm%7Ci2Mj6J9FVXdKDCF0owZynQ.mp4",
    "https://clips-media-assets2.twitch.tv/ANZTtjtlZtdWs7s021s-6w/AT-cm%7CANZTtjtlZtdWs7s021s-6w.mp4",
    "https://clips-media-assets2.twitch.tv/n0JY0Za42er9HEz_Azdhug/39476779349-offset-12222.mp4",
    "https://clips-media-assets2.twitch.tv/ubIoy-GeHQjKkcfKcOQUpQ/39473856597-offset-3766.mp4",
    "https://clips-media-assets2.twitch.tv/Ql9EPfCG9DZ9NxJLOVDhfA/40629800936-offset-1926.mp4",
    "https://clips-media-assets2.twitch.tv/lo0xXPnJRvvU2IM6PCkdlA/AT-cm%7Clo0xXPnJRvvU2IM6PCkdlA.mp4",
    "https://clips-media-assets2.twitch.tv/ZXtL-NmeQlf_I0R2KExDIA/AT-cm%7CZXtL-NmeQlf_I0R2KExDIA.mp4",
    "https://clips-media-assets2.twitch.tv/ls_g8R6MntNy5x_byv1gFw/AT-cm%7Cls_g8R6MntNy5x_byv1gFw.mp4",
    "https://clips-media-assets2.twitch.tv/s6IjBy8UEkvgu4C6cb91Jw/39485365685-offset-3612.mp4",
    "https://clips-media-assets2.twitch.tv/tZmt7pjg--UZd5CdzlX1Qw/39476127589-offset-10220.mp4"

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

