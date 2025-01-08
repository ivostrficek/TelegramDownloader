using Telegram.Bot.Application.ReturnObjects;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;

namespace Telegram.Bot.Application
{
    public class TelegramBotService : ITelegramBotService
    {
        private ITelegramBotClient _botClient;

        public TelegramBotService(ITelegramBotClient telegramBotClient)
        {
            _botClient = telegramBotClient;
        }

        public async Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken)
        {
            var result = await _botClient.SendMessageAsync(chatId, message, cancellationToken: cancellationToken);
        }

        public async Task DeleteMessagesAsync(long chatId, IEnumerable<int> messageIds, CancellationToken cancellationToken)
        {
            var result = await _botClient.DeleteMessagesAsync(chatId, messageIds, cancellationToken: cancellationToken);
        }

        public async Task SendVideoAsync(long chatId, string videoPath, string caption, CancellationToken cancellationToken)
        {
            var sendVideoArgs = new SendVideoArgs(chatId, new InputFile(System.IO.File.ReadAllBytes(videoPath), "video.mp4"))
            {
                Caption = caption
            };

            var result = await _botClient.SendVideoAsync(sendVideoArgs, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ReturnUpdate>> GetUpdatesAsync(int? offset, CancellationToken cancellationToken)
        {
            var resultList = new List<ReturnUpdate>();

            var getUpdatesArgs = new GetUpdatesArgs()
            {
                Offset = offset,
                Timeout = 1000
            };

            var result = await _botClient.GetUpdatesAsync(getUpdatesArgs, cancellationToken: cancellationToken);

            foreach (var update in result)
            {
                var returnUpdate = new ReturnUpdate()
                {
                    UpdateId = update.UpdateId,
                    Message = new ReturnUpdate.ReturnMessage()
                    {
                        MessageId = update.Message.MessageId,
                        Chat = new ReturnUpdate.ReturnMessage.ReturnChat()
                        {
                            Id = update.Message.Chat.Id,
                            Username = update.Message.Chat.Username
                        },
                        Text = update.Message.Text,
                        ChatId = update.Message.Chat.Id
                    }
                };
                resultList.Add(returnUpdate);
            }

            return resultList;
        }
    }
}
