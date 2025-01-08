using FluentResults;
using Microsoft.Playwright;
using System.Text;
using Telegram.Bot.Models;

namespace Telegram.Bot.Downloaders
{
    public class PlayWrightDownloader : IDownloader
    {
        public async Task<Result<DownloaderResponse>> GetVideoSrcAsync(string url)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
                });

                var page = await context.NewPageAsync();

                await page.GotoAsync(url);

                await page.WaitForSelectorAsync("video");
                await page.WaitForSelectorAsync("span[data-e2e='browse-username']");

                var videoElement = await page.QuerySelectorAsync("video");
                var videoSrc = await videoElement.GetAttributeAsync("src");

                var usernameElement = await page.QuerySelectorAsync("span[data-e2e='browse-username']");
                var username = await usernameElement.TextContentAsync();

                if (!string.IsNullOrEmpty(videoSrc))
                {
                    var cookies = await context.CookiesAsync();
                    string cookieHeader = BuildCookieHeader(cookies);

                    var downloadResult = await GetVideoWithCookiesAsync(videoSrc, username, cookieHeader);
                    return downloadResult;
                }

                return Result.Fail(new DownloaderError("Error finding the video source"));
            }
            catch (Exception ex)
            {
                return Result.Fail(new DownloaderError("Error downloading the video", ex));
            }
        }

        private string BuildCookieHeader(IEnumerable<BrowserContextCookiesResult> cookies)
        {
            var cookieHeader = new StringBuilder();
            foreach (var cookie in cookies)
            {
                cookieHeader.Append($"{cookie.Name}={cookie.Value}");
            }

            return cookieHeader.ToString().TrimEnd(';', ' ');
        }

        private async Task<Result<DownloaderResponse>> GetVideoWithCookiesAsync(string videoUrl, string username, string cookies)
        {
            // Extract cookies from PlayWright and download the video
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Cookie", cookies);

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
