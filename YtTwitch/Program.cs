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

List<string> ListaUrl = new List<string>();
List<string> Title = new List<string>();
List<string> StreamerName = new List<string>();

List<string> TempListaUrl = new List<string>()
{
    "https://clips-media-assets2.twitch.tv/1nIkfNbQ6GhHXhkKvDvlug/AT-cm%7C1nIkfNbQ6GhHXhkKvDvlug.mp4",
    "https://clips-media-assets2.twitch.tv/OVfA-WXzZUt7hR6gwShoyg/AT-cm%7COVfA-WXzZUt7hR6gwShoyg.mp4",
};

//DODAJ AKTUALNE CLIPY

static void DownloadUrls(List<string> listaUrl, List<string> Title, List<string> StreamerName)
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
        IReadOnlyCollection<IWebElement> DTitle = wait.Until(i => i.FindElements(By.CssSelector("#root > div > div.Layout-sc-1xcs6mc-0.video-player > div > div > div > div > div:nth-child(4) > div > div:nth-child(1) > div.Layout-sc-1xcs6mc-0.bZVrjx.tw-card > div > div > div.Layout-sc-1xcs6mc-0.bZVrjx.tw-card-body > div > p:nth-child(2)")));
        IReadOnlyCollection<IWebElement> DStreamerName = wait.Until(i => i.FindElements(By.XPath("/html/body/div[1]/div/div[1]/div/div/div/div/div[4]/div/div[1]/div[2]/div/div/div[2]/div/p[2]/a[1]")));
        if (!listaUrl.Contains(Dbtn.First().GetAttribute("href")))
        {
            listaUrl.Add(Dbtn.First().GetAttribute("href"));
            //Title.Add(DTitle.First().Text);
            //StreamerName.Add(DStreamerName.First().Text);
        }

        IReadOnlyCollection<IWebElement> Cbtn = wait.Until(i => i.FindElements(By.XPath("//button[@class = 'absolute top-0 right-0 z-30 flex items-center justify-center w-10 h-10 focus:outline-none']")));
        Cbtn.First().Click();
        i++;
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

/////////////////////////////
DownloadUrls(ListaUrl, Title, StreamerName);
int i = 0;
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
        //new UploadVideo("Test", $"Resclip{i}.mp4").Run().Wait();
        i++;
    }
}
////////////////////////

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
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new string[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private"; // or "private" or "public"
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