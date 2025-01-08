using FluentResults;

namespace Telegram.Bot.Application.Commands
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly ITelegramBotService _telegramBotService;

        public SendMessageCommandHandler(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task<Result> HandleAsync(SendMessageCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _telegramBotService.SendMessageAsync(command.ChatId, command.Message, cancellationToken);
                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("Error while sending message");
            }
        }
    }
}
