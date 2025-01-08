using FluentResults;

namespace Telegram.Bot.Application.Commands
{
    public class DeleteMessagesCommandHandler : ICommandHandler<DeleteMessagesCommand>
    {
        private readonly ITelegramBotService _telegramBotService;

        public DeleteMessagesCommandHandler(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task<Result> HandleAsync(DeleteMessagesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _telegramBotService.DeleteMessagesAsync(command.ChatId, command.MessageIdsToDelete, cancellationToken);
                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("Error while deleting messages");
            }


        }
    }
}
