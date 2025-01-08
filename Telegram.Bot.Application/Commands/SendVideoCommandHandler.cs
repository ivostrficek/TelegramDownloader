using FluentResults;

namespace Telegram.Bot.Application.Commands
{
    public class SendVideoCommandHandler : ICommandHandler<SendVideoCommand>
    {
        private readonly ITelegramBotService _telegramBotService;

        public SendVideoCommandHandler(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task<Result> HandleAsync(SendVideoCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _telegramBotService.SendVideoAsync(command.ChatId, command.VideoPath, command.Caption, cancellationToken);
                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("Error while sending video");
            }

        }
    }
}
