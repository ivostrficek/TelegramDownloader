using FluentResults;
using HtmlAgilityPack;
using System.Text;
using Telegram.Bot.Models;

namespace Telegram.Bot.Downloaders
{
    public class NewTikTokDownloader : IDownloader
    {
        public async Task<Result<DownloaderResponse>> GetVideoSrcAsync(string url)
        {
            using var client = new HttpClient();

            // Set headers
            client.DefaultRequestHeaders.Add("accept", "*/*");
            client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9,es-US;q=0.8,es;q=0.7");
            client.DefaultRequestHeaders.Add("cookie", "_ga=GA1.1.1491836025.1736368437; _ga_ZSF3D6YSLC=GS1.1.1736368437.1.1.1736368526.56.0.0");
            client.DefaultRequestHeaders.Add("hx-current-url", "https://ssstik.io/");
            client.DefaultRequestHeaders.Add("hx-request", "true");
            client.DefaultRequestHeaders.Add("hx-target", "target");
            client.DefaultRequestHeaders.Add("hx-trigger", "_gcaptcha_pt");
            client.DefaultRequestHeaders.Add("origin", "https://ssstik.io");
            client.DefaultRequestHeaders.Add("priority", "u=1, i");
            client.DefaultRequestHeaders.Add("referer", "https://ssstik.io/");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

            // Request body
            var content = new StringContent($"id={url}%2F&locale=en&tt=S1dleXc_", Encoding.UTF8, "application/x-www-form-urlencoded");

            // Send POST request
            var response = await client.PostAsync("https://ssstik.io/abc?url=dl", content);


            var html = await response.Content.ReadAsStringAsync();

            // Parse HTML
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Find the first <a> tag with href
            var firstLink = htmlDoc.DocumentNode.SelectSingleNode("//a[@href]");

            if (firstLink == null)
                Console.WriteLine("No <a> tag with href found.");

            var videoSource = firstLink.GetAttributeValue("href", string.Empty);
            Console.WriteLine($"video URL: {videoSource}");

            // Find the <img> tag with an alt attribute
            var imgTag = htmlDoc.DocumentNode.SelectSingleNode("//img[@alt]");
            if (imgTag == null)
                Console.WriteLine("No <img> tag with alt found.");

            var userName = imgTag.GetAttributeValue("alt", string.Empty);
            Console.WriteLine($"Username: {userName}");

            return await DownloadVideoWithCookies(videoSource, userName);

        }

        private static async Task<Result<DownloaderResponse>> DownloadVideoWithCookies(string videoUrl, string username)
        {
            using var client = new HttpClient();

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
