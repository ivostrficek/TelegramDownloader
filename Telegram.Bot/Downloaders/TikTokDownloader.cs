using FluentResults;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Net;
using Telegram.Bot.Models;

namespace Telegram.Bot.Downloaders
{
    public class TikTokDownloader : IDownloader
    {
        public async Task<Result<DownloaderResponse>> GetVideoSrcAsync(string url)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--headless"); // Run Chrome in headless mode

                using var driver = new ChromeDriver(options);

                driver.Navigate().GoToUrl(url);

                // Wait for the page to fully load (adjust the delay if necessary)
                await Task.Delay(3_000);

                // Find the <video> tag and get the src attribute
                var videoElement = driver.FindElements(By.TagName("video"));

                if (videoElement == null || !videoElement.Any())
                    return Result.Fail(new DownloaderError("Need to login to access"));

                string videoSrc = videoElement.FirstOrDefault(e => e.GetAttribute("src") != null)!.GetAttribute("src");

                // Get the username from the span with data-e2e="browse-username"
                var usernameElement = driver.FindElement(By.CssSelector("span[data-e2e='browse-username']"));

                if (usernameElement == null)
                    return Result.Fail(new DownloaderError("Error finding the username"));

                string username = usernameElement.Text;

                for (int i = 0; i < 5; i++)
                {
                    if (string.IsNullOrEmpty(videoSrc))
                    {
                        Console.WriteLine($"Intento {i}");
                        await Task.Delay(3_000);
                        videoSrc = videoElement.FirstOrDefault(e => e.GetAttribute("src") != null)?.GetAttribute("src");
                    }
                    if (i == 4 && string.IsNullOrEmpty(videoSrc))
                        return Result.Fail(new DownloaderError("Error finding the video source"));
                }

                // Extract cookies from Selenium and download the video
                var downloadResult = await DownloadVideoWithCookies(driver.Manage().Cookies.AllCookies, videoSrc, username);
                return downloadResult;
            }
            catch (Exception ex)
            {
                return Result.Fail(new DownloaderError($"Unknown Error {ex.Message}", ex));
            }
        }

        private static async Task<Result<DownloaderResponse>> DownloadVideoWithCookies(ReadOnlyCollection<OpenQA.Selenium.Cookie> seleniumCookies, string videoUrl, string username)
        {
            using var handler = new HttpClientHandler();

            // Create a cookie container to store the cookies from Selenium
            var cookieContainer = new CookieContainer();

            // Convert Selenium cookies to HttpClient cookies
            foreach (var cookie in seleniumCookies)
            {
                cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            handler.CookieContainer = cookieContainer;

            using var client = new HttpClient(handler);

            try
            {
                HttpResponseMessage response = await client.GetAsync(videoUrl);
                response.EnsureSuccessStatusCode();

                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "videos"));

                var videoPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, "videos", $"{username}_{Guid.NewGuid()}.mp4"));

                // Download the video file and save it
                byte[] videoData = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(videoPath, videoData);
                return Result.Ok(new DownloaderResponse() { Username = username, VideoPath = videoPath });
            }
            catch (Exception ex)
            {
                return Result.Fail(new DownloaderError("Error downloading the video", ex));
            }
        }
    }
}