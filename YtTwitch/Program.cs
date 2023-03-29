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
    "https://clips-media-assets2.twitch.tv/1nIkfNbQ6GhHXhkKvDvlug/AT-cm%7C1nIkfNbQ6GhHXhkKvDvlug.mp4",
    "https://clips-media-assets2.twitch.tv/OVfA-WXzZUt7hR6gwShoyg/AT-cm%7COVfA-WXzZUt7hR6gwShoyg.mp4",
    "https://clips-media-assets2.twitch.tv/RKq3GRvAXr7rFAGx9XunVA/AT-cm%7CRKq3GRvAXr7rFAGx9XunVA.mp4",
    "https://clips-media-assets2.twitch.tv/GUvuj33j0iuEYAUWvjppkQ/39442963445-offset-25164.mp4",
    "https://clips-media-assets2.twitch.tv/CojARo7Z8FNoUSkKOvSygw/39441953669-offset-1904.mp4",
    "https://clips-media-assets2.twitch.tv/q1oaLv3pNkd3Q2je8O-GzQ/AT-cm%7Cq1oaLv3pNkd3Q2je8O-GzQ.mp4",
    "https://clips-media-assets2.twitch.tv/DhKZS-z65ihbYIs7B1Trnw/AT-cm%7CDhKZS-z65ihbYIs7B1Trnw.mp4",
    "https://clips-media-assets2.twitch.tv/dyswnEv7AMIhlnQPo7GRhQ/AT-cm%7CdyswnEv7AMIhlnQPo7GRhQ.mp4",
    "https://clips-media-assets2.twitch.tv/z4SyZDQa_5DOF3Db92oBaQ/39448902037-offset-4976.mp4",
    "https://clips-media-assets2.twitch.tv/d2ffGxHTL-fYHbhgsKDJrw/AT-cm%7Cd2ffGxHTL-fYHbhgsKDJrw.mp4",
    "https://clips-media-assets2.twitch.tv/XGD7E3xJ7lOwCXR5yCtiOg/AT-cm%7CXGD7E3xJ7lOwCXR5yCtiOg.mp4",
    "https://clips-media-assets2.twitch.tv/Leb5sZ4wEq-RM7fAC9oftw/AT-cm%7CLeb5sZ4wEq-RM7fAC9oftw.mp4",
    "https://clips-media-assets2.twitch.tv/W-dSMFO5MZW3ENuxv-aZMw/AT-cm%7CW-dSMFO5MZW3ENuxv-aZMw.mp4",
    "https://clips-media-assets2.twitch.tv/1eknT3u-axiypQjUiMUu2w/AT-cm%7C1eknT3u-axiypQjUiMUu2w.mp4",
    "https://clips-media-assets2.twitch.tv/XXbOFIG_tUDqpKuR8wT5lg/39441953669-offset-14924.mp4",
    "https://clips-media-assets2.twitch.tv/z9lR587i4XPvxiUKfteqZg/AT-cm%7Cz9lR587i4XPvxiUKfteqZg.mp4",
    "https://clips-media-assets2.twitch.tv/fEdYcH4ZAYWVNDsK_01zAA/39442671845-offset-9156.mp4",
    "https://clips-media-assets2.twitch.tv/Ptp5V00OJFFFqW0yt7eiAg/AT-cm%7CPtp5V00OJFFFqW0yt7eiAg.mp4",
    "https://clips-media-assets2.twitch.tv/nVn1LEHwAOObGNuYCOhAhw/AT-cm%7CnVn1LEHwAOObGNuYCOhAhw.mp4",
    "https://clips-media-assets2.twitch.tv/MNyURB8Oe0hHEgfNyss13Q/39444328549-offset-30886.mp4"
};

static void DownloadUrls(List<string> listaUrl, List<string> Title, List<string> StreamerName)
{
    var driver = new ChromeDriver();

    string url = "https://streamscharts.com/clips?time=7-days&language=pl";

    driver.Navigate().GoToUrl(url);

    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    driver.Manage().Window.Maximize();

    System.Threading.Thread.Sleep(1200);
    IReadOnlyCollection<IWebElement> clips = wait.Until(i => i.FindElements(By.ClassName("clip-container")));
    int z = 0;
    foreach (IWebElement element in clips)
    {
        if (z > 11)
        {
            Actions actions = new Actions(driver);
            actions.ScrollByAmount(0, 200);
            actions.Perform();
        }
        System.Threading.Thread.Sleep(1000);
        element.Click();
        System.Threading.Thread.Sleep(1000);
        IReadOnlyCollection<IWebElement> Dbtn = wait.Until(i => i.FindElements(By.XPath("/html/body/main/div[3]/div/div[3]/div/div[2]/div[1]/div/a")));
        IReadOnlyCollection<IWebElement> DStreamerName = wait.Until(i => i.FindElements(By.XPath($"/html/body/main/div[3]/div/div[2]/div[1]/div[1]/div[3]/button[{z + 1}]/div[2]/div")));
        IReadOnlyCollection<IWebElement> DTitle = wait.Until(i => i.FindElements(By.XPath($"/html/body/main/div[3]/div/div[2]/div[1]/div[1]/div[3]/button[{z + 1}]/div[3]/div[2]/div/div[1]")));

        listaUrl.Add(Dbtn.First().GetAttribute("href"));
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
        if (!TempListaUrl.Contains(x))
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(x, $"clip{i}.mp4");

                TempListaUrl.Add(x);

                ChangeResolution($"clip{i}.mp4");
            }
            new UploadVideo($"{StreamerName[i]} - {Title[i]} #shorts #twitch #shoty #irl", $"Resclip{i}.mp4").Run().Wait();

            break;
        }
        i++;
    }
}

Stopwatch Stopwatch = Stopwatch.StartNew();

while (true)
{
    Console.WriteLine(Stopwatch.Elapsed);

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
        video.Snippet.Title = Title;
        video.Snippet.Description = "Shoty dodawane automatycznie co 4h" +
            "Daj suba ziomek 😎";
        video.Snippet.Tags = new string[] { "tag1", "tag2" };
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