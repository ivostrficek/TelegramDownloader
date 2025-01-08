using Telegram.Bot.Application.ReturnObjects;

namespace Telegram.Bot.Application
{
    public interface ITelegramBotService
    {
        Task DeleteMessagesAsync(long chatId, IEnumerable<int> messageIds, CancellationToken cancellationToken);
        Task<IEnumerable<ReturnUpdate>> GetUpdatesAsync(int? offset, CancellationToken cancellationToken);
        Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken);
        Task SendVideoAsync(long chatId, string videoPath, string caption, CancellationToken cancellationToken);
    }
}