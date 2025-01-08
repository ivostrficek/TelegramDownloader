using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Telegram.Bot.Application;
using Telegram.Bot.Application.Commands;
using Telegram.Bot.Application.ReturnObjects;
using Telegram.Bot.Downloaders;

namespace Telegram.Bot
{
    public class App
    {
        private static readonly string tikTokUrlPattern = @"https?:\/\/(www\.|m\.|vm\.)?tiktok\.com\/[A-Za-z0-9\/?=&._-]+";

        private readonly IConfiguration _configuration;
        private readonly IDownloader _downloader;
        private readonly ICommandHandler<GetTelegramUpdatesCommand, IEnumerable<ReturnUpdate>> _getTelegramUpdatesCommandHandler;
        private readonly ICommandHandler<SendMessageCommand> _sendMessageCommandHandler;
        private readonly ICommandHandler<SendVideoCommand> _sendVideoCommandhandler;
        private readonly ICommandHandler<DeleteMessagesCommand> _deleteMessagesCommandHandler;

        public App(IDownloader downloader,
            IConfiguration configuration,
            ICommandHandler<GetTelegramUpdatesCommand, IEnumerable<ReturnUpdate>> getTelegramUpdatesCommandHandler,
            ICommandHandler<SendVideoCommand> sendVideoCommandHandler,
            ICommandHandler<SendMessageCommand> sendMessageCommandHandler,
            ICommandHandler<DeleteMessagesCommand> deleteMessagesCommandHandler)
        {
            _downloader = downloader;
            _configuration = configuration;
            _getTelegramUpdatesCommandHandler = getTelegramUpdatesCommandHandler;
            _sendVideoCommandhandler = sendVideoCommandHandler;
            _sendMessageCommandHandler = sendMessageCommandHandler;
            _deleteMessagesCommandHandler = deleteMessagesCommandHandler;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var vidName = _configuration["VID_NAME"];
            var myId = long.Parse(_configuration["MY_ID"]);

            var updates = await GetUpdatesAsync(cancellationToken);

            while (true)
            {
                if (updates.Count == 0)
                {
                    updates = await GetUpdatesAsync(cancellationToken);
                    continue;
                }

                foreach (var update in updates)
                {
                    var message = update.Message;

                    if (message.Chat.Id != myId)
                    {
                        await SendMessageAsync(message.Chat.Id, "Unautorized", cancellationToken);
                        Console.WriteLine($"Unautorized message from {message.Chat.Id}: {message.Chat.Username}");
                    }

                    if (Regex.IsMatch(message.Text, tikTokUrlPattern))
                    {
                        var downloadResult = await _downloader.GetVideoSrcAsync(message.Text);

                        if (downloadResult.IsFailed)
                        {
                            await SendMessageAsync(myId, downloadResult.Errors.First().Message, cancellationToken);
                            continue;
                        }
                        else
                        {
                            await SendVideoAsync(myId, downloadResult.Value.VideoPath, downloadResult.Value.Username, cancellationToken);

                            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

                            await _deleteMessagesCommandHandler.HandleAsync(new DeleteMessagesCommand()
                            {
                                ChatId = myId,
                                MessageIdsToDelete = [message.MessageId]
                            }, cancellationToken);
                        }

                    }
                    else if (message.Text.Equals(vidName))
                    {
                        await SendVideoAsync(myId, @"videos\cualca.mp4", "", cancellationToken);

                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

                        await _deleteMessagesCommandHandler.HandleAsync(new DeleteMessagesCommand()
                        {
                            ChatId = myId,
                            MessageIdsToDelete = [message.MessageId, message.MessageId + 1]
                        }, cancellationToken);
                    }
                    else
                    {
                        await SendMessageAsync(message.Chat.Id, "Invalid command", cancellationToken);
                    }
                }

                updates = await GetUpdatesAsync(cancellationToken, updates.Max(u => u.UpdateId) + 1);
            }
        }

        private async Task<List<ReturnUpdate>> GetUpdatesAsync(CancellationToken cancellationToken, int? offset = null)
        {
            var command = new GetTelegramUpdatesCommand() { Offset = offset };

            var updatesResult = await _getTelegramUpdatesCommandHandler.HandleAsync(command, cancellationToken);

            if (updatesResult.IsSuccess)
                return updatesResult.Value.ToList();
            else
                return [];
        }

        private async Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken)
        {
            var sendMessageResult = await _sendMessageCommandHandler.HandleAsync(new SendMessageCommand()
            {
                ChatId = chatId,
                Message = message
            }, cancellationToken);

            if (sendMessageResult.IsFailed)
                Console.WriteLine($"Error sending message to {chatId}: {sendMessageResult.Errors.First().Message}");
        }

        private async Task SendVideoAsync(long chatId, string videoPath, string caption, CancellationToken cancellationToken)
        {
            var sendVideoResult = await _sendVideoCommandhandler.HandleAsync(new SendVideoCommand()
            {
                ChatId = chatId,
                VideoPath = videoPath,
                Caption = caption
            }, cancellationToken);

            if (sendVideoResult.IsFailed)
                Console.WriteLine($"Error sending video to {chatId}: {sendVideoResult.Errors.First().Message}");
        }
    }
}
