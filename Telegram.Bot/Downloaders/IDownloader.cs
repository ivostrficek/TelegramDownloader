using FluentResults;
using Telegram.Bot.Models;

namespace Telegram.Bot.Downloaders
{
    public interface IDownloader
    {
        Task<Result<DownloaderResponse>> GetVideoSrcAsync(string url);
    }
}
